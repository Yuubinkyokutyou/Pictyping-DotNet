using Microsoft.EntityFrameworkCore;
using Pictyping.Core.Entities;

namespace Pictyping.Infrastructure.Data;

public class PictypingDbContext : DbContext
{
    public PictypingDbContext(DbContextOptions<PictypingDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<OnesideTwoPlayerTypingMatch> OnesideTwoPlayerTypingMatches { get; set; }
    public DbSet<PenaltyRiskAction> PenaltyRiskActions { get; set; }
    public DbSet<OmniAuthIdentity> OmniAuthIdentities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Users テーブルのマッピング（既存のRailsスキーマに合わせる）
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email").HasDefaultValue("").IsRequired();
            entity.Property(e => e.EncryptedPassword).HasColumnName("encrypted_password").HasDefaultValue("").IsRequired();
            entity.Property(e => e.ResetPasswordToken).HasColumnName("reset_password_token");
            entity.Property(e => e.ResetPasswordSentAt).HasColumnName("reset_password_sent_at");
            entity.Property(e => e.RememberCreatedAt).HasColumnName("remember_created_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").IsRequired();
            entity.Property(e => e.Guest).HasColumnName("guest").HasDefaultValue(false);
            entity.Property(e => e.PlayfabId).HasColumnName("playfabId");  // Rails schema exact name
            entity.Property(e => e.Name).HasColumnName("name").HasDefaultValue("noname").IsRequired();
            entity.Property(e => e.Rating).HasColumnName("rating").IsRequired();
            entity.Property(e => e.OnlineGameBanDate).HasColumnName("online_game_ban_date");
            entity.Property(e => e.Admin).HasColumnName("admin").HasDefaultValue(false);

            entity.HasIndex(e => e.Email).HasDatabaseName("index_users_on_email").IsUnique();
            entity.HasIndex(e => e.ResetPasswordToken).HasDatabaseName("index_users_on_reset_password_token").IsUnique();
            entity.HasIndex(e => e.Rating).HasDatabaseName("index_users_on_rating");
        });

        // OnesideTwoPlayerTypingMatches テーブルのマッピング
        modelBuilder.Entity<OnesideTwoPlayerTypingMatch>(entity =>
        {
            entity.ToTable("oneside_two_player_typing_matches");
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BattleDataJson).HasColumnName("battle_data_json")
                .HasColumnType("jsonb").HasDefaultValue("{}").IsRequired();
            entity.Property(e => e.MatchId).HasColumnName("match_id").IsRequired();
            entity.Property(e => e.RegisterId).HasColumnName("register_id").IsRequired();
            entity.Property(e => e.EnemyId).HasColumnName("enemy_id");
            entity.Property(e => e.EnemyStartedRating).HasColumnName("enemy_started_rating").IsRequired();
            entity.Property(e => e.StartedRating).HasColumnName("started_rating").IsRequired();
            entity.Property(e => e.IsFinished).HasColumnName("is_finished").HasDefaultValue(false).IsRequired();
            entity.Property(e => e.FinishedRating).HasColumnName("finished_rating");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").IsRequired();
            entity.Property(e => e.BattleStatus).HasColumnName("battle_status");

            entity.HasOne(e => e.RegisterUser)
                .WithMany(u => u.TypingMatchesAsRegister)
                .HasForeignKey(e => e.RegisterId);

            entity.HasOne(e => e.EnemyUser)
                .WithMany(u => u.TypingMatchesAsEnemy)
                .HasForeignKey(e => e.EnemyId);

            entity.HasIndex(e => e.RegisterId).HasDatabaseName("index_oneside_two_player_typing_matches_on_register_id");
            entity.HasIndex(e => e.EnemyId).HasDatabaseName("index_oneside_two_player_typing_matches_on_enemy_id");
            entity.HasIndex(e => e.MatchId).HasDatabaseName("index_oneside_two_player_typing_matches_on_match_id");
        });

        // PenaltyRiskActions テーブルのマッピング
        modelBuilder.Entity<PenaltyRiskAction>(entity =>
        {
            entity.ToTable("penalty_risk_actions");
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").IsRequired();
            entity.Property(e => e.ActionType).HasColumnName("action_type").IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.PenaltyRiskActions)
                .HasForeignKey(e => e.UserId);

            entity.HasIndex(e => e.UserId).HasDatabaseName("index_penalty_risk_actions_on_user_id");
        });

        // OmniAuthIdentities テーブルのマッピング
        modelBuilder.Entity<OmniAuthIdentity>(entity =>
        {
            entity.ToTable("omni_auth_identities");
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Provider).HasColumnName("provider");
            entity.Property(e => e.Uid).HasColumnName("uid");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.OmniAuthIdentities)
                .HasForeignKey(e => e.UserId);

            entity.HasIndex(e => e.UserId).HasDatabaseName("index_omni_auth_identities_on_user_id");
            entity.HasIndex(e => new { e.Provider, e.Uid }).HasDatabaseName("index_omni_auth_identities_on_provider_and_uid").IsUnique();
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;

            if (entityEntry.State == EntityState.Added)
            {
                ((BaseEntity)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}