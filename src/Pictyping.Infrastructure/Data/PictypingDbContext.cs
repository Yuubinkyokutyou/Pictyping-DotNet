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
            entity.Property(e => e.Email).HasColumnName("email").HasDefaultValue("");
            entity.Property(e => e.EncryptedPassword).HasColumnName("encrypted_password").HasDefaultValue("");
            entity.Property(e => e.ResetPasswordToken).HasColumnName("reset_password_token");
            entity.Property(e => e.ResetPasswordSentAt).HasColumnName("reset_password_sent_at");
            entity.Property(e => e.RememberCreatedAt).HasColumnName("remember_created_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.Guest).HasColumnName("guest").HasDefaultValue(false);
            entity.Property(e => e.PlayFabId).HasColumnName("play_fab_id");
            entity.Property(e => e.Rating).HasColumnName("rating").HasDefaultValue(1200);
            entity.Property(e => e.Admin).HasColumnName("admin").HasDefaultValue(false);
            entity.Property(e => e.DisplayName).HasColumnName("display_name");

            entity.HasIndex(e => e.Email).HasDatabaseName("index_users_on_email");
            entity.HasIndex(e => e.ResetPasswordToken).HasDatabaseName("index_users_on_reset_password_token");
            entity.HasIndex(e => e.Rating).HasDatabaseName("index_users_on_rating");
        });

        // OnesideTwoPlayerTypingMatches テーブルのマッピング
        modelBuilder.Entity<OnesideTwoPlayerTypingMatch>(entity =>
        {
            entity.ToTable("oneside_two_player_typing_matches");
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.EnemyUserId).HasColumnName("enemy_user_id");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.Accuracy).HasColumnName("accuracy");
            entity.Property(e => e.TypeSpeed).HasColumnName("type_speed");
            entity.Property(e => e.MissCount).HasColumnName("miss_count");
            entity.Property(e => e.BattleTime).HasColumnName("battle_time");
            entity.Property(e => e.QuestionContents).HasColumnName("question_contents");
            entity.Property(e => e.InputContents).HasColumnName("input_contents");
            entity.Property(e => e.MissTypeContents).HasColumnName("miss_type_contents");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.BattleStatus).HasColumnName("battle_status");

            entity.HasOne(e => e.User)
                .WithMany(u => u.TypingMatchesAsPlayer)
                .HasForeignKey(e => e.UserId);

            entity.HasOne(e => e.EnemyUser)
                .WithMany(u => u.TypingMatchesAsEnemy)
                .HasForeignKey(e => e.EnemyUserId);

            entity.HasIndex(e => e.UserId).HasDatabaseName("index_oneside_two_player_typing_matches_on_user_id");
        });

        // PenaltyRiskActions テーブルのマッピング
        modelBuilder.Entity<PenaltyRiskAction>(entity =>
        {
            entity.ToTable("penalty_risk_actions");
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ReporterId).HasColumnName("reporter_id");
            entity.Property(e => e.PlayFabId).HasColumnName("play_fab_id");
            entity.Property(e => e.MatchId).HasColumnName("match_id");
            entity.Property(e => e.ActionType).HasColumnName("action_type");
            entity.Property(e => e.Detail).HasColumnName("detail");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasIndex(e => e.ActionType).HasDatabaseName("index_penalty_risk_actions_on_action_type");
            entity.HasIndex(e => e.PlayFabId).HasDatabaseName("index_penalty_risk_actions_on_play_fab_id");
        });

        // OmniAuthIdentities テーブルのマッピング
        modelBuilder.Entity<OmniAuthIdentity>(entity =>
        {
            entity.ToTable("omni_auth_identities");
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Provider).HasColumnName("provider");
            entity.Property(e => e.Uid).HasColumnName("uid");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(e => e.User)
                .WithMany(u => u.OmniAuthIdentities)
                .HasForeignKey(e => e.UserId);

            entity.HasIndex(e => e.UserId).HasDatabaseName("index_omni_auth_identities_on_user_id");
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