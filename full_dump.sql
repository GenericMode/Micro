--
-- PostgreSQL database cluster dump
--

SET default_transaction_read_only = off;

SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;

--
-- Roles
--

CREATE ROLE postgres;
ALTER ROLE postgres WITH SUPERUSER INHERIT CREATEROLE CREATEDB LOGIN REPLICATION BYPASSRLS PASSWORD 'SCRAM-SHA-256$4096:3zNj0Pe8Obb/ckqNfW9O+g==$3yXTk9PjQARXvYv1bbDM8I8M2d23cLLemrydEeGyPGs=:Qb716miLl0TNXz6MlfwDCXvkCdM+rxLXKXzn++z1JJQ=';

--
-- User Configurations
--








--
-- Databases
--

--
-- Database "template1" dump
--

\connect template1

--
-- PostgreSQL database dump
--

-- Dumped from database version 17.2 (Debian 17.2-1.pgdg120+1)
-- Dumped by pg_dump version 17.2 (Debian 17.2-1.pgdg120+1)

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

--
-- PostgreSQL database dump complete
--

--
-- Database "postgres" dump
--

\connect postgres

--
-- PostgreSQL database dump
--

-- Dumped from database version 17.2 (Debian 17.2-1.pgdg120+1)
-- Dumped by pg_dump version 17.2 (Debian 17.2-1.pgdg120+1)

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

--
-- Name: pgcrypto; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS pgcrypto WITH SCHEMA public;


--
-- Name: EXTENSION pgcrypto; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION pgcrypto IS 'cryptographic functions';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: Order; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Order" (
    "Id" uuid DEFAULT gen_random_uuid() NOT NULL,
    "CompanyName" text NOT NULL,
    "ProductId" integer NOT NULL,
    "ProductName" text NOT NULL,
    "ProductQuantity" integer NOT NULL,
    "OrderDate" timestamp with time zone,
    "StatusId" integer NOT NULL
);


ALTER TABLE public."Order" OWNER TO postgres;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;

--
-- Data for Name: Order; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Order" ("Id", "CompanyName", "ProductId", "ProductName", "ProductQuantity", "OrderDate", "StatusId") FROM stdin;
28c05bd7-1e6a-4cb8-8ec1-0a680ae897a5	AutoMapCompany	32	Tires Michellene 32	2	2025-01-26 14:45:08.729+00	1
0f9f3f0f-e16b-45c1-8207-7367506c3321	New Company	15	MacBook Air 2024 rev.1	2	2025-01-07 18:49:19.449+00	5
37e094c4-7189-4148-80bb-7b7c95f10480	New Company 2	15	MacBook Air 2024 rev.1	4	2025-01-26 22:41:30.256+00	5
c39119b0-b73b-40a3-8681-cb710b885e2d	Very Big Company99	27	27Product	3	2025-02-10 01:13:41.602+00	1
176b85d0-0b1c-4cd9-a4f3-ca6b1f03f193	Rabbit Company	411	Good Product	3	2025-02-16 15:20:42.461+00	1
c6dc9553-4cc2-4f2a-bc7a-eb6a955e37e2	New PC Company3	12	MacBookPro2025	14	2025-01-07 00:20:33.532+00	1
89b88376-d347-4ea9-aa14-136d5503ffce	April company	56	New April Product	10	2025-04-18 15:52:11.199+00	5
d3872393-a45f-433e-9aac-b723463486cc	FlyCompany	12	MacBook 2025 rev.	11	2025-04-25 14:45:08.729+00	1
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20250106225330_NewMigration	9.0.0
20250106235049_NewMigration2	9.0.0
20250106235846_FixUUIDGeneration	9.0.0
\.


--
-- Name: Order PK_Order; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Order"
    ADD CONSTRAINT "PK_Order" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- PostgreSQL database dump complete
--

--
-- Database "postgreswh" dump
--

--
-- PostgreSQL database dump
--

-- Dumped from database version 17.2 (Debian 17.2-1.pgdg120+1)
-- Dumped by pg_dump version 17.2 (Debian 17.2-1.pgdg120+1)

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

--
-- Name: postgreswh; Type: DATABASE; Schema: -; Owner: postgres
--

CREATE DATABASE postgreswh WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8';


ALTER DATABASE postgreswh OWNER TO postgres;

\connect postgreswh

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
-- Name: Product; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Product" (
    "Id" uuid DEFAULT gen_random_uuid() NOT NULL,
    "ProductId" integer NOT NULL,
    "ProductName" text NOT NULL,
    "ProductStoredQuantity" integer NOT NULL,
    "ProductBookedQuantity" integer,
    "ProductStorage" integer NOT NULL
);


ALTER TABLE public."Product" OWNER TO postgres;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;

--
-- Data for Name: Product; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Product" ("Id", "ProductId", "ProductName", "ProductStoredQuantity", "ProductBookedQuantity", "ProductStorage") FROM stdin;
b3972c76-df3c-4dd4-8d32-9688d5f2ec7c	1	TestProduct	560	0	1
6f26b4a7-3127-445e-b722-bf169a9f6837	12	MacBook 2025 rev.	750	31	2
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20250115201230_MigrationName	9.0.0
\.


--
-- Name: Product PK_Product; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Product"
    ADD CONSTRAINT "PK_Product" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- PostgreSQL database dump complete
--

--
-- PostgreSQL database cluster dump complete
--

