--
-- PostgreSQL database dump
--

\restrict 3OzQvsGaF4QyKegRisRhq4wfut1nzD6qmNqDewYFNGkY56hsrM4eafOPK5fTGaT

-- Dumped from database version 18.2 (Ubuntu 18.2-1.pgdg24.04+1)
-- Dumped by pg_dump version 18.2 (Ubuntu 18.2-1.pgdg24.04+1)

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
-- Name: app_users; Type: TABLE; Schema: public; Owner: thaseven_dev
--

CREATE TABLE public.app_users (
    id integer NOT NULL,
    username character varying(20) NOT NULL,
    email character varying(20) NOT NULL,
    password character varying NOT NULL
);


ALTER TABLE public.app_users OWNER TO thaseven_dev;

--
-- Name: app_users_id_seq; Type: SEQUENCE; Schema: public; Owner: thaseven_dev
--

CREATE SEQUENCE public.app_users_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.app_users_id_seq OWNER TO thaseven_dev;

--
-- Name: app_users_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: thaseven_dev
--

ALTER SEQUENCE public.app_users_id_seq OWNED BY public.app_users.id;


--
-- Name: game_records; Type: TABLE; Schema: public; Owner: thaseven_dev
--

CREATE TABLE public.game_records (
    id integer NOT NULL,
    user_email character varying(20) NOT NULL,
    map_name character varying(20) NOT NULL,
    season character varying NOT NULL,
    rank character varying NOT NULL,
    rank_division integer,
    rank_percentage integer,
    hero_1 character varying NOT NULL,
    hero_2 character varying,
    hero_3 character varying,
    match_result character varying NOT NULL,
    team_notes character varying,
    enemy_team_notes character varying,
    team_ban_1 character varying,
    team_ban_2 character varying,
    enemy_team_ban_1 character varying,
    enemy_team_ban_2 character varying
);


ALTER TABLE public.game_records OWNER TO thaseven_dev;

--
-- Name: game_records_id_seq; Type: SEQUENCE; Schema: public; Owner: thaseven_dev
--

CREATE SEQUENCE public.game_records_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.game_records_id_seq OWNER TO thaseven_dev;

--
-- Name: game_records_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: thaseven_dev
--

ALTER SEQUENCE public.game_records_id_seq OWNED BY public.game_records.id;


--
-- Name: hero_list; Type: TABLE; Schema: public; Owner: thaseven_dev
--

CREATE TABLE public.hero_list (
    id integer NOT NULL,
    name character varying(20) NOT NULL,
    role character varying(20) NOT NULL
);


ALTER TABLE public.hero_list OWNER TO thaseven_dev;

--
-- Name: hero_list_id_seq; Type: SEQUENCE; Schema: public; Owner: thaseven_dev
--

CREATE SEQUENCE public.hero_list_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.hero_list_id_seq OWNER TO thaseven_dev;

--
-- Name: hero_list_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: thaseven_dev
--

ALTER SEQUENCE public.hero_list_id_seq OWNED BY public.hero_list.id;


--
-- Name: map_list; Type: TABLE; Schema: public; Owner: thaseven_dev
--

CREATE TABLE public.map_list (
    id integer NOT NULL,
    name character varying(20) NOT NULL,
    mode character varying(20) NOT NULL,
    mode_id integer
);


ALTER TABLE public.map_list OWNER TO thaseven_dev;

--
-- Name: map_list_id_seq; Type: SEQUENCE; Schema: public; Owner: thaseven_dev
--

CREATE SEQUENCE public.map_list_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.map_list_id_seq OWNER TO thaseven_dev;

--
-- Name: map_list_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: thaseven_dev
--

ALTER SEQUENCE public.map_list_id_seq OWNED BY public.map_list.id;


--
-- Name: app_users id; Type: DEFAULT; Schema: public; Owner: thaseven_dev
--

ALTER TABLE ONLY public.app_users ALTER COLUMN id SET DEFAULT nextval('public.app_users_id_seq'::regclass);


--
-- Name: game_records id; Type: DEFAULT; Schema: public; Owner: thaseven_dev
--

ALTER TABLE ONLY public.game_records ALTER COLUMN id SET DEFAULT nextval('public.game_records_id_seq'::regclass);


--
-- Name: hero_list id; Type: DEFAULT; Schema: public; Owner: thaseven_dev
--

ALTER TABLE ONLY public.hero_list ALTER COLUMN id SET DEFAULT nextval('public.hero_list_id_seq'::regclass);


--
-- Name: map_list id; Type: DEFAULT; Schema: public; Owner: thaseven_dev
--

ALTER TABLE ONLY public.map_list ALTER COLUMN id SET DEFAULT nextval('public.map_list_id_seq'::regclass);


--
-- Data for Name: app_users; Type: TABLE DATA; Schema: public; Owner: thaseven_dev
--

COPY public.app_users (id, username, email, password) FROM stdin;
1	mimmo	mimmo@mimmo.com	AQAAAAIAAYagAAAAEKkWe3YZYa5fk6v+ecnnqkhfJAnZNiKvfgaLo671QqVZ+r9smdzDY2nt6fsc4e1Ccw==
2	mimmo23	mimmomm@mimmmooo.com	AQAAAAIAAYagAAAAENcL0NQ48hES0aLrOGPueItZRgXrvxjRpcWLU7A4nnBb86M3kDsUESn6CXOHkZxhTQ==
\.


--
-- Data for Name: game_records; Type: TABLE DATA; Schema: public; Owner: thaseven_dev
--

COPY public.game_records (id, user_email, map_name, season, rank, rank_division, rank_percentage, hero_1, hero_2, hero_3, match_result, team_notes, enemy_team_notes, team_ban_1, team_ban_2, enemy_team_ban_1, enemy_team_ban_2) FROM stdin;
\.


--
-- Data for Name: hero_list; Type: TABLE DATA; Schema: public; Owner: thaseven_dev
--

COPY public.hero_list (id, name, role) FROM stdin;
1	Tracer	Damage
2	Reaper	Damage
3	Widowmaker	Damage
4	Phara	Damage
5	Reinhardt	Tank
6	Mercy	Support
7	Torbjörn	Damage
8	Hanzo	Damage
9	Winston	Tank
10	Zenyatta	Support
11	Bastion	Damage
12	Symmetra	Damage
13	Zarya	Tank
14	Cassidy	Damage
15	Soldier: 76	Damage
16	Lúcio	Support
17	Roadhog	Tank
18	Junkrat	Damage
19	D.Va	Tank
20	Mei	Damage
21	Genji	Damage
22	Ana	Support
23	Sombra	Damage
24	Orisa	Tank
25	Doomfist	Tank
26	Moira	Support
27	Brigitte	Support
28	Wrecking Ball	Tank
29	Ashe	Damage
30	Baptiste	Support
31	Sigma	Tank
32	Echo	Damage
33	Sojourn	Damage
34	Junker Queen	Tank
35	Kiriko	Support
36	Ramattra	Tank
37	Lifeweaver	Support
38	Illari	Support
39	Mauga	Tank
40	Venture	Damage
41	Juno	Support
42	Hazard	Tank
43	Freja	Damage
44	Wuyang	Support
45	Vendetta	Damage
46	Domina	Tank
47	Emre	Damage
48	Mizuki	Support
49	Anran	Damage
50	Jetpack Cat	Support
\.


--
-- Data for Name: map_list; Type: TABLE DATA; Schema: public; Owner: thaseven_dev
--

COPY public.map_list (id, name, mode, mode_id) FROM stdin;
2	King's Row	Hybrid	2
3	Numbani	Hybrid	2
4	Dorado	Escort	1
5	Hollywood	Hybrid	2
6	Lijiang Tower	Control	3
7	Ilios	Control	3
8	Nepal	Control	3
9	Route 66	Escort	1
10	Eichenwalde	Hybrid	2
11	Oasis	Control	3
12	Junkertown	Escort	1
13	Blizzard World	Hybrid	2
14	Rialto	Escort	1
15	Busan	Control	3
16	Havana	Escort	1
17	New Queen Street	Push	4
18	Circuit Royal	Escort	1
19	Colosseo	Push	4
20	Midtown	Hybrid	2
21	Paraíso	Hybrid	2
22	Esperança	Push	4
23	Shambali Monastery	Escort	1
24	Antarctic Peninsula	Control	3
25	New Junk City	Flashpoint	5
26	Suravasa	Flashpoint	5
27	Samoa	Control	3
28	Runasapi	Push	4
29	Aatlis	Flashpoint	5
\.


--
-- Name: app_users_id_seq; Type: SEQUENCE SET; Schema: public; Owner: thaseven_dev
--

SELECT pg_catalog.setval('public.app_users_id_seq', 2, true);


--
-- Name: game_records_id_seq; Type: SEQUENCE SET; Schema: public; Owner: thaseven_dev
--

SELECT pg_catalog.setval('public.game_records_id_seq', 1, false);


--
-- Name: hero_list_id_seq; Type: SEQUENCE SET; Schema: public; Owner: thaseven_dev
--

SELECT pg_catalog.setval('public.hero_list_id_seq', 50, true);


--
-- Name: map_list_id_seq; Type: SEQUENCE SET; Schema: public; Owner: thaseven_dev
--

SELECT pg_catalog.setval('public.map_list_id_seq', 29, true);


--
-- Name: app_users app_users_email_key; Type: CONSTRAINT; Schema: public; Owner: thaseven_dev
--

ALTER TABLE ONLY public.app_users
    ADD CONSTRAINT app_users_email_key UNIQUE (email);


--
-- Name: app_users app_users_pkey; Type: CONSTRAINT; Schema: public; Owner: thaseven_dev
--

ALTER TABLE ONLY public.app_users
    ADD CONSTRAINT app_users_pkey PRIMARY KEY (id);


--
-- Name: app_users app_users_username_key; Type: CONSTRAINT; Schema: public; Owner: thaseven_dev
--

ALTER TABLE ONLY public.app_users
    ADD CONSTRAINT app_users_username_key UNIQUE (username);


--
-- Name: game_records game_records_pkey; Type: CONSTRAINT; Schema: public; Owner: thaseven_dev
--

ALTER TABLE ONLY public.game_records
    ADD CONSTRAINT game_records_pkey PRIMARY KEY (id);


--
-- Name: hero_list hero_list_name_key; Type: CONSTRAINT; Schema: public; Owner: thaseven_dev
--

ALTER TABLE ONLY public.hero_list
    ADD CONSTRAINT hero_list_name_key UNIQUE (name);


--
-- Name: hero_list hero_list_pkey; Type: CONSTRAINT; Schema: public; Owner: thaseven_dev
--

ALTER TABLE ONLY public.hero_list
    ADD CONSTRAINT hero_list_pkey PRIMARY KEY (id);


--
-- Name: map_list map_list_name_key; Type: CONSTRAINT; Schema: public; Owner: thaseven_dev
--

ALTER TABLE ONLY public.map_list
    ADD CONSTRAINT map_list_name_key UNIQUE (name);


--
-- Name: map_list map_list_pkey; Type: CONSTRAINT; Schema: public; Owner: thaseven_dev
--

ALTER TABLE ONLY public.map_list
    ADD CONSTRAINT map_list_pkey PRIMARY KEY (id);


--
-- Name: game_records fk_enemy_team_ban_hero_1; Type: FK CONSTRAINT; Schema: public; Owner: thaseven_dev
--

ALTER TABLE ONLY public.game_records
    ADD CONSTRAINT fk_enemy_team_ban_hero_1 FOREIGN KEY (enemy_team_ban_1) REFERENCES public.hero_list(name);


--
-- Name: game_records fk_enemy_team_ban_hero_2; Type: FK CONSTRAINT; Schema: public; Owner: thaseven_dev
--

ALTER TABLE ONLY public.game_records
    ADD CONSTRAINT fk_enemy_team_ban_hero_2 FOREIGN KEY (enemy_team_ban_2) REFERENCES public.hero_list(name);


--
-- Name: game_records fk_hero_1; Type: FK CONSTRAINT; Schema: public; Owner: thaseven_dev
--

ALTER TABLE ONLY public.game_records
    ADD CONSTRAINT fk_hero_1 FOREIGN KEY (hero_1) REFERENCES public.hero_list(name);


--
-- Name: game_records fk_hero_2; Type: FK CONSTRAINT; Schema: public; Owner: thaseven_dev
--

ALTER TABLE ONLY public.game_records
    ADD CONSTRAINT fk_hero_2 FOREIGN KEY (hero_2) REFERENCES public.hero_list(name);


--
-- Name: game_records fk_hero_3; Type: FK CONSTRAINT; Schema: public; Owner: thaseven_dev
--

ALTER TABLE ONLY public.game_records
    ADD CONSTRAINT fk_hero_3 FOREIGN KEY (hero_3) REFERENCES public.hero_list(name);


--
-- Name: game_records fk_team_ban_hero_1; Type: FK CONSTRAINT; Schema: public; Owner: thaseven_dev
--

ALTER TABLE ONLY public.game_records
    ADD CONSTRAINT fk_team_ban_hero_1 FOREIGN KEY (team_ban_1) REFERENCES public.hero_list(name);


--
-- Name: game_records fk_team_ban_hero_2; Type: FK CONSTRAINT; Schema: public; Owner: thaseven_dev
--

ALTER TABLE ONLY public.game_records
    ADD CONSTRAINT fk_team_ban_hero_2 FOREIGN KEY (team_ban_2) REFERENCES public.hero_list(name);


--
-- PostgreSQL database dump complete
--

\unrestrict 3OzQvsGaF4QyKegRisRhq4wfut1nzD6qmNqDewYFNGkY56hsrM4eafOPK5fTGaT

