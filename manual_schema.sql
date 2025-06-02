-- Rails Compatible Schema Creation for ASP.NET Migration Test

-- Enable necessary extensions
CREATE EXTENSION IF NOT EXISTS plpgsql;

-- Users table
CREATE TABLE public.users (
    id bigserial PRIMARY KEY,
    email character varying DEFAULT '' NOT NULL,
    encrypted_password character varying DEFAULT '' NOT NULL,
    reset_password_token character varying,
    reset_password_sent_at timestamp(6) without time zone,
    remember_created_at timestamp(6) without time zone,
    created_at timestamp(6) without time zone NOT NULL,
    updated_at timestamp(6) without time zone NOT NULL,
    guest boolean DEFAULT false,
    "playfabId" character varying,
    name character varying DEFAULT 'noname' NOT NULL,
    rating integer NOT NULL,
    online_game_ban_date timestamp(6) without time zone,
    admin boolean DEFAULT false
);

-- OmniAuth Identities table
CREATE TABLE public.omni_auth_identities (
    id bigserial PRIMARY KEY,
    provider character varying,
    uid character varying,
    email character varying,
    user_id bigint NOT NULL,
    created_at timestamp(6) without time zone NOT NULL,
    updated_at timestamp(6) without time zone NOT NULL
);

-- Oneside Two Player Typing Matches table
CREATE TABLE public.oneside_two_player_typing_matches (
    id bigserial PRIMARY KEY,
    battle_data_json jsonb DEFAULT '{}' NOT NULL,
    match_id character varying NOT NULL,
    register_id integer NOT NULL,
    enemy_id integer,
    enemy_started_rating integer NOT NULL,
    started_rating integer NOT NULL,
    is_finished boolean DEFAULT false NOT NULL,
    finished_rating integer,
    created_at timestamp(6) without time zone NOT NULL,
    updated_at timestamp(6) without time zone NOT NULL,
    battle_status integer
);

-- Penalty Risk Actions table
CREATE TABLE public.penalty_risk_actions (
    id bigserial PRIMARY KEY,
    user_id bigint NOT NULL,
    created_at timestamp(6) without time zone NOT NULL,
    updated_at timestamp(6) without time zone NOT NULL,
    action_type integer NOT NULL
);

-- Indexes
CREATE UNIQUE INDEX index_users_on_email ON public.users USING btree (email);
CREATE UNIQUE INDEX index_users_on_reset_password_token ON public.users USING btree (reset_password_token);
CREATE INDEX index_users_on_rating ON public.users USING btree (rating);

CREATE INDEX index_omni_auth_identities_on_user_id ON public.omni_auth_identities USING btree (user_id);
CREATE UNIQUE INDEX index_omni_auth_identities_on_provider_and_uid ON public.omni_auth_identities USING btree (provider, uid);

CREATE INDEX index_oneside_two_player_typing_matches_on_register_id ON public.oneside_two_player_typing_matches USING btree (register_id);
CREATE INDEX index_oneside_two_player_typing_matches_on_enemy_id ON public.oneside_two_player_typing_matches USING btree (enemy_id);
CREATE INDEX index_oneside_two_player_typing_matches_on_match_id ON public.oneside_two_player_typing_matches USING btree (match_id);

CREATE INDEX index_penalty_risk_actions_on_user_id ON public.penalty_risk_actions USING btree (user_id);

-- Foreign Keys
ALTER TABLE ONLY public.omni_auth_identities
    ADD CONSTRAINT fk_rails_omni_auth_identities_user_id FOREIGN KEY (user_id) REFERENCES public.users(id);

ALTER TABLE ONLY public.penalty_risk_actions
    ADD CONSTRAINT fk_rails_penalty_risk_actions_user_id FOREIGN KEY (user_id) REFERENCES public.users(id);