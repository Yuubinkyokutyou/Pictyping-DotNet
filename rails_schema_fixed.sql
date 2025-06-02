--
-- PostgreSQL database dump
--

-- Dumped from database version 17.0 (Debian 17.0-1.pgdg120+1)
-- Dumped by pg_dump version 17.0 (Debian 17.0-1.pgdg120+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: ar_internal_metadata; Type: TABLE; Schema: public; Owner: root
--

CREATE TABLE public.ar_internal_metadata (
    key character varying NOT NULL,
    value character varying,
    created_at timestamp(6) without time zone NOT NULL,
    updated_at timestamp(6) without time zone NOT NULL
);


ALTER TABLE public.ar_internal_metadata OWNER TO postgres;

--
-- Name: omni_auth_identities; Type: TABLE; Schema: public; Owner: root
--

CREATE TABLE public.omni_auth_identities (
    id bigint NOT NULL,
    provider character varying,
    uid character varying,
    email character varying,
    user_id bigint NOT NULL,
    created_at timestamp(6) without time zone NOT NULL,
    updated_at timestamp(6) without time zone NOT NULL
);


ALTER TABLE public.omni_auth_identities OWNER TO postgres;

--
-- Name: omni_auth_identities_id_seq; Type: SEQUENCE; Schema: public; Owner: root
--

CREATE SEQUENCE public.omni_auth_identities_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.omni_auth_identities_id_seq OWNER TO postgres;

--
-- Name: omni_auth_identities_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: root
--

ALTER SEQUENCE public.omni_auth_identities_id_seq OWNED BY public.omni_auth_identities.id;


--
-- Name: oneside_two_player_typing_matches; Type: TABLE; Schema: public; Owner: root
--

CREATE TABLE public.oneside_two_player_typing_matches (
    id bigint NOT NULL,
    battle_data_json jsonb DEFAULT '"{}"'::jsonb NOT NULL,
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


ALTER TABLE public.oneside_two_player_typing_matches OWNER TO postgres;

--
-- Name: oneside_two_player_typing_matches_id_seq; Type: SEQUENCE; Schema: public; Owner: root
--

CREATE SEQUENCE public.oneside_two_player_typing_matches_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.oneside_two_player_typing_matches_id_seq OWNER TO postgres;

--
-- Name: oneside_two_player_typing_matches_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: root
--

ALTER SEQUENCE public.oneside_two_player_typing_matches_id_seq OWNED BY public.oneside_two_player_typing_matches.id;


--
-- Name: penalty_risk_actions; Type: TABLE; Schema: public; Owner: root
--

CREATE TABLE public.penalty_risk_actions (
    id bigint NOT NULL,
    user_id bigint NOT NULL,
    created_at timestamp(6) without time zone NOT NULL,
    updated_at timestamp(6) without time zone NOT NULL,
    action_type integer NOT NULL
);


ALTER TABLE public.penalty_risk_actions OWNER TO postgres;

--
-- Name: penalty_risk_actions_id_seq; Type: SEQUENCE; Schema: public; Owner: root
--

CREATE SEQUENCE public.penalty_risk_actions_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.penalty_risk_actions_id_seq OWNER TO postgres;

--
-- Name: penalty_risk_actions_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: root
--

ALTER SEQUENCE public.penalty_risk_actions_id_seq OWNED BY public.penalty_risk_actions.id;


--
-- Name: schema_migrations; Type: TABLE; Schema: public; Owner: root
--

CREATE TABLE public.schema_migrations (
    version character varying NOT NULL
);


ALTER TABLE public.schema_migrations OWNER TO postgres;

--
-- Name: users; Type: TABLE; Schema: public; Owner: root
--

CREATE TABLE public.users (
    id bigint NOT NULL,
    email character varying DEFAULT ''::character varying NOT NULL,
    encrypted_password character varying DEFAULT ''::character varying NOT NULL,
    reset_password_token character varying,
    reset_password_sent_at timestamp(6) without time zone,
    remember_created_at timestamp(6) without time zone,
    created_at timestamp(6) without time zone NOT NULL,
    updated_at timestamp(6) without time zone NOT NULL,
    guest boolean DEFAULT false,
    "playfabId" character varying,
    name character varying DEFAULT 'noname'::character varying NOT NULL,
    rating integer NOT NULL,
    online_game_ban_date timestamp(6) without time zone,
    admin boolean DEFAULT false
);


ALTER TABLE public.users OWNER TO postgres;

--
-- Name: users_id_seq; Type: SEQUENCE; Schema: public; Owner: root
--

CREATE SEQUENCE public.users_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.users_id_seq OWNER TO postgres;

--
-- Name: users_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: root
--

ALTER SEQUENCE public.users_id_seq OWNED BY public.users.id;


--
-- Name: omni_auth_identities id; Type: DEFAULT; Schema: public; Owner: root
--

ALTER TABLE ONLY public.omni_auth_identities ALTER COLUMN id SET DEFAULT nextval('public.omni_auth_identities_id_seq'::regclass);


--
-- Name: oneside_two_player_typing_matches id; Type: DEFAULT; Schema: public; Owner: root
--

ALTER TABLE ONLY public.oneside_two_player_typing_matches ALTER COLUMN id SET DEFAULT nextval('public.oneside_two_player_typing_matches_id_seq'::regclass);


--
-- Name: penalty_risk_actions id; Type: DEFAULT; Schema: public; Owner: root
--

ALTER TABLE ONLY public.penalty_risk_actions ALTER COLUMN id SET DEFAULT nextval('public.penalty_risk_actions_id_seq'::regclass);


--
-- Name: users id; Type: DEFAULT; Schema: public; Owner: root
--

ALTER TABLE ONLY public.users ALTER COLUMN id SET DEFAULT nextval('public.users_id_seq'::regclass);


--
-- Name: ar_internal_metadata ar_internal_metadata_pkey; Type: CONSTRAINT; Schema: public; Owner: root
--

ALTER TABLE ONLY public.ar_internal_metadata
    ADD CONSTRAINT ar_internal_metadata_pkey PRIMARY KEY (key);


--
-- Name: omni_auth_identities omni_auth_identities_pkey; Type: CONSTRAINT; Schema: public; Owner: root
--

ALTER TABLE ONLY public.omni_auth_identities
    ADD CONSTRAINT omni_auth_identities_pkey PRIMARY KEY (id);


--
-- Name: oneside_two_player_typing_matches oneside_two_player_typing_matches_pkey; Type: CONSTRAINT; Schema: public; Owner: root
--

ALTER TABLE ONLY public.oneside_two_player_typing_matches
    ADD CONSTRAINT oneside_two_player_typing_matches_pkey PRIMARY KEY (id);


--
-- Name: penalty_risk_actions penalty_risk_actions_pkey; Type: CONSTRAINT; Schema: public; Owner: root
--

ALTER TABLE ONLY public.penalty_risk_actions
    ADD CONSTRAINT penalty_risk_actions_pkey PRIMARY KEY (id);


--
-- Name: schema_migrations schema_migrations_pkey; Type: CONSTRAINT; Schema: public; Owner: root
--

ALTER TABLE ONLY public.schema_migrations
    ADD CONSTRAINT schema_migrations_pkey PRIMARY KEY (version);


--
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: root
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- Name: index_omni_auth_identities_on_provider_and_uid; Type: INDEX; Schema: public; Owner: root
--

CREATE UNIQUE INDEX index_omni_auth_identities_on_provider_and_uid ON public.omni_auth_identities USING btree (provider, uid);


--
-- Name: index_omni_auth_identities_on_user_id; Type: INDEX; Schema: public; Owner: root
--

CREATE INDEX index_omni_auth_identities_on_user_id ON public.omni_auth_identities USING btree (user_id);


--
-- Name: index_oneside_two_player_typing_matches_on_enemy_id; Type: INDEX; Schema: public; Owner: root
--

CREATE INDEX index_oneside_two_player_typing_matches_on_enemy_id ON public.oneside_two_player_typing_matches USING btree (enemy_id);


--
-- Name: index_oneside_two_player_typing_matches_on_match_id; Type: INDEX; Schema: public; Owner: root
--

CREATE INDEX index_oneside_two_player_typing_matches_on_match_id ON public.oneside_two_player_typing_matches USING btree (match_id);


--
-- Name: index_oneside_two_player_typing_matches_on_register_id; Type: INDEX; Schema: public; Owner: root
--

CREATE INDEX index_oneside_two_player_typing_matches_on_register_id ON public.oneside_two_player_typing_matches USING btree (register_id);


--
-- Name: index_penalty_risk_actions_on_user_id; Type: INDEX; Schema: public; Owner: root
--

CREATE INDEX index_penalty_risk_actions_on_user_id ON public.penalty_risk_actions USING btree (user_id);


--
-- Name: index_users_on_email; Type: INDEX; Schema: public; Owner: root
--

CREATE UNIQUE INDEX index_users_on_email ON public.users USING btree (email);


--
-- Name: index_users_on_rating; Type: INDEX; Schema: public; Owner: root
--

CREATE INDEX index_users_on_rating ON public.users USING btree (rating);


--
-- Name: index_users_on_reset_password_token; Type: INDEX; Schema: public; Owner: root
--

CREATE UNIQUE INDEX index_users_on_reset_password_token ON public.users USING btree (reset_password_token);


--
-- Name: omni_auth_identities fk_rails_17235d7b06; Type: FK CONSTRAINT; Schema: public; Owner: root
--

ALTER TABLE ONLY public.omni_auth_identities
    ADD CONSTRAINT fk_rails_17235d7b06 FOREIGN KEY (user_id) REFERENCES public.users(id);


--
-- Name: penalty_risk_actions fk_rails_6a4ddba6f8; Type: FK CONSTRAINT; Schema: public; Owner: root
--

ALTER TABLE ONLY public.penalty_risk_actions
    ADD CONSTRAINT fk_rails_6a4ddba6f8 FOREIGN KEY (user_id) REFERENCES public.users(id);


--
-- PostgreSQL database dump complete
--

