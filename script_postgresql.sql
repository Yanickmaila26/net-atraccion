-- ============================================================
--  ServicioAtraccionDB  →  PostgreSQL
--  Generado: 2026-04-28
--  Estrategia:
--    • Limpia SOLO tablas de atracciones / booking / pagos
--    • Conserva y enriquece: usuarios, roles, categorías,
--      subcategorías, tags, locations, idiomas, inclusiones,
--      ticket categories, estados de catálogo
-- ============================================================

-- ============================================================
-- 0. EXTENSIÓN UUID
-- ============================================================
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

DO $$
BEGIN
  IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'yanick_admin') THEN
    CREATE ROLE yanick_admin LOGIN PASSWORD 'Admin@Atrac2026!';
  END IF;

  IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'yanick_api') THEN
    CREATE ROLE yanick_api LOGIN PASSWORD 'Api@Atrac2026!';
  END IF;

  IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'yanick_readonly') THEN
    CREATE ROLE yanick_readonly LOGIN PASSWORD 'ReadOnly@Atrac2026!';
  END IF;

  IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'yanick_booking') THEN
    CREATE ROLE yanick_booking LOGIN PASSWORD 'Booking@Atrac2026!';
  END IF;

  IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'yanick_partner') THEN
    CREATE ROLE yanick_partner LOGIN PASSWORD 'Partner@Atrac2026!';
  END IF;
END $$;

-- ============================================================
-- 1. TABLAS DE CATÁLOGO / LOOKUP
-- ============================================================

-- Language
CREATE TABLE IF NOT EXISTS language (
    id          SMALLSERIAL PRIMARY KEY,
    iso_code    VARCHAR(5)  NOT NULL UNIQUE,
    name        VARCHAR(50) NOT NULL,
    is_active   BOOLEAN     NOT NULL DEFAULT TRUE
);

-- MediaType
CREATE TABLE IF NOT EXISTS media_type (
    id   SMALLINT PRIMARY KEY,
    name VARCHAR(20) NOT NULL UNIQUE
);

-- BookingStatus
CREATE TABLE IF NOT EXISTS booking_status (
    id   SMALLINT PRIMARY KEY,
    name VARCHAR(20) NOT NULL UNIQUE
);

-- PaymentMethodType
CREATE TABLE IF NOT EXISTS payment_method_type (
    id   SMALLINT PRIMARY KEY,
    name VARCHAR(30) NOT NULL UNIQUE
);

-- PaymentStatusType
CREATE TABLE IF NOT EXISTS payment_status_type (
    id   SMALLINT PRIMARY KEY,
    name VARCHAR(20) NOT NULL UNIQUE
);

-- ReviewCriteria
CREATE TABLE IF NOT EXISTS review_criteria (
    id   SMALLINT PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE
);

-- TicketCategory
CREATE TABLE IF NOT EXISTS ticket_category (
    id            UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    name          VARCHAR(50) NOT NULL,
    name_en       VARCHAR(80) NOT NULL,
    age_range_min SMALLINT    NOT NULL,
    age_range_max SMALLINT    NOT NULL,
    sort_order    SMALLINT    NOT NULL DEFAULT 0
);

-- ============================================================
-- 2. ROLES Y USUARIOS
-- ============================================================

CREATE TABLE IF NOT EXISTS role (
    id          UUID         PRIMARY KEY DEFAULT gen_random_uuid(),
    name        VARCHAR(50)  NOT NULL UNIQUE,
    description TEXT,
    created_at  TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    updated_at  TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS users (
    id            UUID         PRIMARY KEY DEFAULT gen_random_uuid(),
    email         VARCHAR(256) NOT NULL UNIQUE,
    password_hash TEXT         NOT NULL,
    is_active     BOOLEAN      NOT NULL DEFAULT TRUE,
    created_at    TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    updated_at    TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS user_role (
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    role_id UUID NOT NULL REFERENCES role(id),
    PRIMARY KEY (user_id, role_id)
);

-- ============================================================
-- 3. UBICACIONES
-- ============================================================

CREATE TABLE IF NOT EXISTS locations (
    id           UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    name         VARCHAR(100) NOT NULL,
    type         VARCHAR(50)  NOT NULL,   -- Country | State | City | District
    parent_id    UUID         REFERENCES locations(id),
    country_code CHAR(2),
    created_at   TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    updated_at   TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

-- ============================================================
-- 4. CATEGORÍAS Y SUBCATEGORÍAS
-- ============================================================

CREATE TABLE IF NOT EXISTS category (
    id         UUID         PRIMARY KEY DEFAULT gen_random_uuid(),
    slug       VARCHAR(100) NOT NULL UNIQUE,
    name       VARCHAR(100) NOT NULL,
    icon_url   TEXT,
    sort_order SMALLINT     NOT NULL DEFAULT 0,
    is_active  BOOLEAN      NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS category_translation (
    id          UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    category_id UUID        NOT NULL REFERENCES category(id) ON DELETE CASCADE,
    language_id SMALLINT    NOT NULL REFERENCES language(id),
    name        VARCHAR(100) NOT NULL,
    UNIQUE (category_id, language_id)
);

CREATE TABLE IF NOT EXISTS subcategory (
    id          UUID         PRIMARY KEY DEFAULT gen_random_uuid(),
    category_id UUID         NOT NULL REFERENCES category(id) ON DELETE CASCADE,
    slug        VARCHAR(100) NOT NULL UNIQUE,
    name        VARCHAR(100) NOT NULL,
    icon_url    TEXT,
    sort_order  SMALLINT     NOT NULL DEFAULT 0,
    is_active   BOOLEAN      NOT NULL DEFAULT TRUE,
    created_at  TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    updated_at  TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS subcategory_translation (
    id             UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    subcategory_id UUID        NOT NULL REFERENCES subcategory(id) ON DELETE CASCADE,
    language_id    SMALLINT    NOT NULL REFERENCES language(id),
    name           VARCHAR(100) NOT NULL,
    UNIQUE (subcategory_id, language_id)
);

-- ============================================================
-- 5. TAGS E INCLUSIONES
-- ============================================================

CREATE TABLE IF NOT EXISTS tag (
    id   UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(50) NOT NULL,
    slug VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS inclusion_item (
    id           UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    icon_slug    VARCHAR(50),
    default_text TEXT        NOT NULL,
    language_id  SMALLINT    NOT NULL REFERENCES language(id),
    created_at   TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ============================================================
-- 6. CLIENTE (perfil extendido del usuario)
-- ============================================================

CREATE TABLE IF NOT EXISTS client (
    id              UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id         UUID        NOT NULL UNIQUE REFERENCES users(id) ON DELETE CASCADE,
    first_name      VARCHAR(100) NOT NULL,
    last_name       VARCHAR(100) NOT NULL,
    phone           VARCHAR(20),
    birth_date      DATE,
    nationality     VARCHAR(100),
    document_type   VARCHAR(20),
    document_number VARCHAR(50),
    location_id     UUID        REFERENCES locations(id),
    avatar_url      TEXT,
    preferred_lang  SMALLINT    REFERENCES language(id),
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    deleted_at      TIMESTAMPTZ
);

-- ============================================================
-- 7. ATRACCIONES Y PRODUCTOS
-- ============================================================

CREATE TABLE IF NOT EXISTS attraction (
    id                UUID         PRIMARY KEY DEFAULT gen_random_uuid(),
    location_id       UUID         NOT NULL REFERENCES locations(id),
    subcategory_id    UUID         NOT NULL REFERENCES subcategory(id),
    slug              VARCHAR(200) NOT NULL UNIQUE,
    name              VARCHAR(150) NOT NULL,
    description_short VARCHAR(255),
    description_full  TEXT,
    address           TEXT,
    latitude          NUMERIC(9,6),
    longitude         NUMERIC(9,6),
    meeting_point     TEXT,
    rating_average    NUMERIC(3,2) NOT NULL DEFAULT 0,
    rating_count      INTEGER      NOT NULL DEFAULT 0,
    min_age           SMALLINT,
    max_group_size    SMALLINT,
    difficulty_level  VARCHAR(20)  CHECK (difficulty_level IN ('easy','moderate','hard')),
    is_active         BOOLEAN      NOT NULL DEFAULT TRUE,
    is_published      BOOLEAN      NOT NULL DEFAULT FALSE,
    managed_by_id     UUID         REFERENCES users(id),
    created_at        TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    updated_at        TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    deleted_at        TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS attraction_translation (
    id                UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    attraction_id     UUID        NOT NULL REFERENCES attraction(id) ON DELETE CASCADE,
    language_id       SMALLINT    NOT NULL REFERENCES language(id),
    name              VARCHAR(150) NOT NULL,
    description_short VARCHAR(255),
    description_full  TEXT,
    meeting_point     TEXT,
    UNIQUE (attraction_id, language_id)
);

CREATE TABLE IF NOT EXISTS attraction_tag (
    attraction_id UUID NOT NULL REFERENCES attraction(id) ON DELETE CASCADE,
    tag_id        UUID NOT NULL REFERENCES tag(id) ON DELETE CASCADE,
    PRIMARY KEY (attraction_id, tag_id)
);

CREATE TABLE IF NOT EXISTS attraction_inclusion (
    attraction_id    UUID        NOT NULL REFERENCES attraction(id) ON DELETE CASCADE,
    inclusion_item_id UUID       NOT NULL REFERENCES inclusion_item(id),
    type             VARCHAR(20) NOT NULL CHECK (type IN ('included','not_included','optional','bring')),
    PRIMARY KEY (attraction_id, inclusion_item_id)
);

CREATE TABLE IF NOT EXISTS attraction_language (
    id            UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    attraction_id UUID        NOT NULL REFERENCES attraction(id) ON DELETE CASCADE,
    language_id   SMALLINT    NOT NULL REFERENCES language(id),
    guide_type    VARCHAR(20) NOT NULL CHECK (guide_type IN ('live','audio','written','app')),
    UNIQUE (attraction_id, language_id, guide_type)
);

CREATE TABLE IF NOT EXISTS attraction_media (
    id            UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    attraction_id UUID        NOT NULL REFERENCES attraction(id) ON DELETE CASCADE,
    media_type_id SMALLINT    NOT NULL REFERENCES media_type(id),
    url           TEXT        NOT NULL,
    thumbnail_url TEXT,
    title         VARCHAR(150),
    language_id   SMALLINT    REFERENCES language(id),
    is_main       BOOLEAN     NOT NULL DEFAULT FALSE,
    sort_order    SMALLINT    NOT NULL DEFAULT 0,
    file_size_kb  INTEGER,
    duration_secs INTEGER,
    created_at    TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ============================================================
-- 8. PRODUCTOS / OPCIONES
-- ============================================================

CREATE TABLE IF NOT EXISTS product_option (
    id                   UUID         PRIMARY KEY DEFAULT gen_random_uuid(),
    attraction_id        UUID         NOT NULL REFERENCES attraction(id) ON DELETE CASCADE,
    slug                 VARCHAR(150) NOT NULL,
    title                VARCHAR(150) NOT NULL,
    description          TEXT,
    duration_minutes     INTEGER,
    duration_description VARCHAR(100),
    cancel_policy_hours  INTEGER      NOT NULL DEFAULT 24,
    cancel_policy_text   TEXT,
    max_group_size       SMALLINT,
    min_participants     SMALLINT     NOT NULL DEFAULT 1,
    is_active            BOOLEAN      NOT NULL DEFAULT TRUE,
    is_private           BOOLEAN      NOT NULL DEFAULT FALSE,
    sort_order           SMALLINT     NOT NULL DEFAULT 0,
    created_at           TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    updated_at           TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UNIQUE (attraction_id, slug)
);

CREATE TABLE IF NOT EXISTS product_translation (
    id                   UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    product_id           UUID        NOT NULL REFERENCES product_option(id) ON DELETE CASCADE,
    language_id          SMALLINT    NOT NULL REFERENCES language(id),
    title                VARCHAR(150) NOT NULL,
    description          TEXT,
    duration_description VARCHAR(100),
    cancel_policy_text   TEXT,
    UNIQUE (product_id, language_id)
);

CREATE TABLE IF NOT EXISTS product_inclusion (
    product_id        UUID        NOT NULL REFERENCES product_option(id) ON DELETE CASCADE,
    inclusion_item_id UUID        NOT NULL REFERENCES inclusion_item(id),
    type              VARCHAR(20) NOT NULL CHECK (type IN ('included','not_included','optional','bring')),
    PRIMARY KEY (product_id, inclusion_item_id)
);

-- ============================================================
-- 9. PRECIOS Y DISPONIBILIDAD
-- ============================================================

CREATE TABLE IF NOT EXISTS price_tier (
    id                 UUID         PRIMARY KEY DEFAULT gen_random_uuid(),
    product_id         UUID         NOT NULL REFERENCES product_option(id) ON DELETE CASCADE,
    ticket_category_id UUID         NOT NULL REFERENCES ticket_category(id),
    price              NUMERIC(12,2) NOT NULL CHECK (price >= 0),
    currency_code      CHAR(3)      NOT NULL DEFAULT 'USD',
    is_active          BOOLEAN      NOT NULL DEFAULT TRUE,
    created_at         TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    updated_at         TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS product_schedule_template (
    id               UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    product_id       UUID        NOT NULL REFERENCES product_option(id) ON DELETE CASCADE,
    name             VARCHAR(100) NOT NULL,
    monday           BOOLEAN     NOT NULL DEFAULT FALSE,
    tuesday          BOOLEAN     NOT NULL DEFAULT FALSE,
    wednesday        BOOLEAN     NOT NULL DEFAULT FALSE,
    thursday         BOOLEAN     NOT NULL DEFAULT FALSE,
    friday           BOOLEAN     NOT NULL DEFAULT FALSE,
    saturday         BOOLEAN     NOT NULL DEFAULT FALSE,
    sunday           BOOLEAN     NOT NULL DEFAULT FALSE,
    valid_from       DATE        NOT NULL,
    valid_to         DATE,
    default_capacity SMALLINT    NOT NULL,
    is_active        BOOLEAN     NOT NULL DEFAULT TRUE,
    notes            TEXT,
    created_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at       TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS product_schedule_time (
    id                UUID     PRIMARY KEY DEFAULT gen_random_uuid(),
    template_id       UUID     NOT NULL REFERENCES product_schedule_template(id) ON DELETE CASCADE,
    start_time        TIME     NOT NULL,
    end_time          TIME,
    capacity_override SMALLINT,
    sort_order        SMALLINT NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS availability_slot (
    id                 UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    product_id         UUID        NOT NULL REFERENCES product_option(id),
    slot_date          DATE        NOT NULL,
    start_time         TIME        NOT NULL,
    end_time           TIME,
    capacity_total     SMALLINT    NOT NULL CHECK (capacity_total > 0),
    capacity_available SMALLINT    NOT NULL CHECK (capacity_available >= 0),
    is_active          BOOLEAN     NOT NULL DEFAULT TRUE,
    notes              TEXT,
    created_at         TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at         TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE (product_id, slot_date, start_time),
    CHECK (capacity_available <= capacity_total)
);

-- ============================================================
-- 10. ITINERARIOS / TOUR STOPS / AUDIO GUIDE
-- ============================================================

CREATE TABLE IF NOT EXISTS tour_itinerary (
    id            UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    attraction_id UUID        NOT NULL REFERENCES attraction(id) ON DELETE CASCADE,
    language_id   SMALLINT    NOT NULL REFERENCES language(id),
    title         VARCHAR(150) NOT NULL,
    description   TEXT,
    created_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at    TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS tour_stop (
    id             UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    itinerary_id   UUID        NOT NULL REFERENCES tour_itinerary(id) ON DELETE CASCADE,
    stop_number    SMALLINT    NOT NULL,
    title          VARCHAR(150) NOT NULL,
    description    TEXT,
    latitude       NUMERIC(9,6),
    longitude      NUMERIC(9,6),
    duration_mins  SMALLINT,
    admission_type VARCHAR(20) CHECK (admission_type IN ('included','optional','not_included','excluded','bring')),
    UNIQUE (itinerary_id, stop_number)
);

CREATE TABLE IF NOT EXISTS tour_stop_media (
    id            UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    stop_id       UUID        NOT NULL REFERENCES tour_stop(id) ON DELETE CASCADE,
    media_type_id SMALLINT    NOT NULL REFERENCES media_type(id),
    url           TEXT        NOT NULL,
    sort_order    SMALLINT    NOT NULL DEFAULT 0,
    created_at    TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS audio_guide (
    id                 UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    attraction_id      UUID        NOT NULL REFERENCES attraction(id) ON DELETE CASCADE,
    language_id        SMALLINT    NOT NULL REFERENCES language(id),
    title              VARCHAR(150) NOT NULL,
    description        TEXT,
    total_duration_secs INTEGER,
    is_active          BOOLEAN     NOT NULL DEFAULT TRUE,
    created_at         TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at         TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE (attraction_id, language_id)
);

CREATE TABLE IF NOT EXISTS audio_guide_stop (
    id            UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    audio_guide_id UUID       NOT NULL REFERENCES audio_guide(id) ON DELETE CASCADE,
    stop_number   SMALLINT    NOT NULL,
    title         VARCHAR(150) NOT NULL,
    description   TEXT,
    audio_url     TEXT        NOT NULL,
    duration_secs INTEGER,
    latitude      NUMERIC(9,6),
    longitude     NUMERIC(9,6),
    image_url     TEXT,
    UNIQUE (audio_guide_id, stop_number)
);

-- ============================================================
-- 11. RESERVAS Y PAGOS
-- ============================================================

CREATE TABLE IF NOT EXISTS booking (
    id             UUID         PRIMARY KEY DEFAULT gen_random_uuid(),
    pnr_code       VARCHAR(8)   NOT NULL UNIQUE,
    user_id        UUID         NOT NULL REFERENCES users(id),
    slot_id        UUID         NOT NULL REFERENCES availability_slot(id),
    status_id      SMALLINT     NOT NULL REFERENCES booking_status(id),
    total_amount   NUMERIC(12,2) NOT NULL CHECK (total_amount >= 0),
    currency_code  CHAR(3)      NOT NULL DEFAULT 'USD',
    language_id    SMALLINT     REFERENCES language(id),
    notes          TEXT,
    internal_notes TEXT,
    created_at     TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    updated_at     TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    cancelled_at   TIMESTAMPTZ,
    cancel_reason  TEXT
);

CREATE TABLE IF NOT EXISTS booking_detail (
    id              UUID         PRIMARY KEY DEFAULT gen_random_uuid(),
    booking_id      UUID         NOT NULL REFERENCES booking(id) ON DELETE CASCADE,
    price_tier_id   UUID         NOT NULL REFERENCES price_tier(id),
    first_name      VARCHAR(100) NOT NULL,
    last_name       VARCHAR(100) NOT NULL,
    document_type   VARCHAR(20),
    document_number VARCHAR(50)  NOT NULL,
    quantity        SMALLINT     NOT NULL CHECK (quantity > 0),
    unit_price      NUMERIC(12,2) NOT NULL,
    created_at      TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS payment (
    id                      UUID         PRIMARY KEY DEFAULT gen_random_uuid(),
    booking_id              UUID         NOT NULL REFERENCES booking(id),
    transaction_external_id VARCHAR(100),
    payment_method_id       SMALLINT     NOT NULL REFERENCES payment_method_type(id),
    status_id               SMALLINT     NOT NULL REFERENCES payment_status_type(id),
    amount                  NUMERIC(12,2) NOT NULL CHECK (amount >= 0),
    currency_code           CHAR(3)      NOT NULL DEFAULT 'USD',
    gateway_response        TEXT,
    paid_at                 TIMESTAMPTZ,
    refunded_at             TIMESTAMPTZ,
    refund_reason           TEXT,
    created_at              TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    updated_at              TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

-- ============================================================
-- 12. RESEÑAS
-- ============================================================

CREATE TABLE IF NOT EXISTS review (
    id            UUID         PRIMARY KEY DEFAULT gen_random_uuid(),
    booking_id    UUID         NOT NULL UNIQUE REFERENCES booking(id),
    user_id       UUID         NOT NULL REFERENCES users(id),
    overall_score NUMERIC(3,2) NOT NULL CHECK (overall_score BETWEEN 1 AND 5),
    title         VARCHAR(150),
    comment       TEXT,
    response      TEXT,
    responded_at  TIMESTAMPTZ,
    is_visible    BOOLEAN      NOT NULL DEFAULT TRUE,
    is_verified   BOOLEAN      NOT NULL DEFAULT FALSE,
    language_id   SMALLINT     REFERENCES language(id),
    created_at    TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    updated_at    TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS review_rating (
    id          UUID     PRIMARY KEY DEFAULT gen_random_uuid(),
    review_id   UUID     NOT NULL REFERENCES review(id) ON DELETE CASCADE,
    criteria_id SMALLINT NOT NULL REFERENCES review_criteria(id),
    score       SMALLINT NOT NULL CHECK (score BETWEEN 1 AND 5),
    UNIQUE (review_id, criteria_id)
);

CREATE TABLE IF NOT EXISTS review_media (
    id         UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    review_id  UUID        NOT NULL REFERENCES review(id) ON DELETE CASCADE,
    url        TEXT        NOT NULL,
    sort_order SMALLINT    NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ============================================================
-- 13. AUDITORÍA
-- ============================================================

CREATE TABLE IF NOT EXISTS audit_log (
    id         UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    table_name VARCHAR(100) NOT NULL,
    record_id  UUID        NOT NULL,
    action     VARCHAR(10)  NOT NULL CHECK (action IN ('INSERT','UPDATE','DELETE')),
    changed_by VARCHAR(256),
    changed_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    old_values JSONB,
    new_values JSONB,
    ip_address VARCHAR(45),
    user_agent VARCHAR(500),
    endpoint   VARCHAR(255)
);

-- ============================================================
-- ============================================================
-- DATOS SEED
-- ============================================================
-- ============================================================

-- ============================================================
-- S1. LIMPIEZA ORDENADA (solo tablas de atracciones/booking)
-- ============================================================
TRUNCATE TABLE
    review_media, review_rating, review,
    payment, booking_detail, booking,
    availability_slot,
    product_schedule_time, product_schedule_template,
    price_tier,
    product_inclusion, product_translation, product_option,
    audio_guide_stop, audio_guide,
    tour_stop_media, tour_stop, tour_itinerary,
    attraction_inclusion, attraction_tag,
    attraction_media, attraction_language, attraction_translation,
    attraction
CASCADE;

-- ============================================================
-- S2. CATÁLOGO / LOOKUPS
-- ============================================================

INSERT INTO language (iso_code, name, is_active) VALUES
    ('es', 'Español',   TRUE),
    ('en', 'English',   TRUE),
    ('pt', 'Português', TRUE),
    ('fr', 'Français',  TRUE),
    ('de', 'Deutsch',   TRUE),
    ('zh', 'Chinese',   TRUE)
ON CONFLICT (iso_code) DO NOTHING;

INSERT INTO media_type (id, name) VALUES
    (1, 'image'),
    (2, 'video'),
    (3, 'audio'),
    (4, 'document')
ON CONFLICT (id) DO NOTHING;

INSERT INTO booking_status (id, name) VALUES
    (1, 'pending'),
    (2, 'confirmed'),
    (3, 'cancelled'),
    (4, 'completed'),
    (5, 'no_show'),
    (6, 'refunded')
ON CONFLICT (id) DO NOTHING;

INSERT INTO payment_method_type (id, name) VALUES
    (1, 'credit_card'),
    (2, 'debit_card'),
    (3, 'paypal'),
    (4, 'bank_transfer'),
    (5, 'cash'),
    (6, 'crypto')
ON CONFLICT (id) DO NOTHING;

INSERT INTO payment_status_type (id, name) VALUES
    (1, 'pending'),
    (2, 'authorized'),
    (3, 'captured'),
    (4, 'failed'),
    (5, 'refunded'),
    (6, 'disputed')
ON CONFLICT (id) DO NOTHING;

INSERT INTO review_criteria (id, name) VALUES
    (1, 'Guia / Experiencia'),
    (2, 'Relación calidad-precio'),
    (3, 'Organización'),
    (4, 'Seguridad'),
    (5, 'Puntualidad'),
    (6, 'Limpieza')
ON CONFLICT (id) DO NOTHING;

INSERT INTO ticket_category (id, name, name_en, age_range_min, age_range_max, sort_order) VALUES
    ('a1b2c3d4-e5f6-4a1b-8c9d-0e1f2a3b4c5d', 'Adulto',       'Adult (18-64)',    18, 64,  1),
    ('b2c3d4e5-f6a1-4b2c-9d0e-1f2a3b4c5d6e', 'Adulto Mayor', 'Senior (65+)',     65, 100, 2),
    ('c3d4e5f6-a1b2-4c3d-0e1f-2a3b4c5d6e7f', 'Niños',        'Junior (5-12)',     5, 12,  3),
    ('d4e5f6a1-b2c3-4d4e-1f2a-3b4c5d6e7f8a', 'Bebé',         'Infant (0-4)',      0,  4,  4),
    ('e5f6a1b2-c3d4-4e5f-2a3b-4c5d6e7f8a9b', 'Joven',        'Youth (13-17)',    13, 17,  5),
    ('f6a1b2c3-d4e5-4f6a-3b4c-5d6e7f8a9b0c', 'Familia',      'Family (2A+2N)',    0, 99,  6)
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- S3. ROLES
-- ============================================================
INSERT INTO role (id, name, description) VALUES
    ('a1111111-1111-1111-1111-111111111111', 'Admin',   'Administración total: usuarios, configuración y auditoría'),
    ('b2222222-2222-2222-2222-222222222222', 'Partner', 'Gestión de catálogo propio, precios, disponibilidad e itinerarios'),
    ('c3333333-3333-3333-3333-333333333333', 'Client',  'Búsqueda de catálogo, reserva y proceso de pago de atracciones'),
    ('d4444444-4444-4444-4444-444444444444', 'Staff',   'Soporte operativo: check-in, validación de tickets y asistencia'),
    ('e5555555-5555-5555-5555-555555555555', 'Reviewer','Moderación de reseñas y contenido generado por usuarios')
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- S4. USUARIOS  (password: secreto123 – BCrypt)
-- ============================================================
DO $$
DECLARE
    v_hash TEXT := '$2a$11$TVBFSiEsaZ4cKgM.H.lTU.cvHVH4F6y9JxAplLsudLc0F53cPWMVe';
BEGIN
    INSERT INTO users (id, email, password_hash, is_active) VALUES
        ('10000000-0000-0000-0000-000000000001', 'yanick_admin@atraccion.com',   v_hash, TRUE),
        ('20000000-0000-0000-0000-000000000002', 'yanick_partner@atraccion.com', v_hash, TRUE),
        ('30000000-0000-0000-0000-000000000003', 'yanick_client@atraccion.com',  v_hash, TRUE),
        ('40000000-0000-0000-0000-000000000004', 'yanick_staff@atraccion.com',   v_hash, TRUE),
        ('50000000-0000-0000-0000-000000000005', 'yanick_reviewer@atraccion.com',v_hash, TRUE),
        -- Clientes demo adicionales
        ('60000000-0000-0000-0000-000000000006', 'maria.garcia@demo.com',        v_hash, TRUE),
        ('70000000-0000-0000-0000-000000000007', 'carlos.mendez@demo.com',       v_hash, TRUE),
        ('80000000-0000-0000-0000-000000000008', 'ana.torres@demo.com',          v_hash, TRUE),
        ('90000000-0000-0000-0000-000000000009', 'john.smith@demo.com',          v_hash, TRUE),
        ('a0000000-0000-0000-0000-000000000010', 'sophie.dupont@demo.com',       v_hash, TRUE)
    ON CONFLICT (id) DO NOTHING;

    INSERT INTO user_role (user_id, role_id) VALUES
        ('10000000-0000-0000-0000-000000000001', 'a1111111-1111-1111-1111-111111111111'),
        ('20000000-0000-0000-0000-000000000002', 'b2222222-2222-2222-2222-222222222222'),
        ('30000000-0000-0000-0000-000000000003', 'c3333333-3333-3333-3333-333333333333'),
        ('40000000-0000-0000-0000-000000000004', 'd4444444-4444-4444-4444-444444444444'),
        ('50000000-0000-0000-0000-000000000005', 'e5555555-5555-5555-5555-555555555555'),
        ('60000000-0000-0000-0000-000000000006', 'c3333333-3333-3333-3333-333333333333'),
        ('70000000-0000-0000-0000-000000000007', 'c3333333-3333-3333-3333-333333333333'),
        ('80000000-0000-0000-0000-000000000008', 'c3333333-3333-3333-3333-333333333333'),
        ('90000000-0000-0000-0000-000000000009', 'c3333333-3333-3333-3333-333333333333'),
        ('a0000000-0000-0000-0000-000000000010', 'c3333333-3333-3333-3333-333333333333')
    ON CONFLICT DO NOTHING;
END $$;

-- ============================================================
-- S5. UBICACIONES (Ecuador + más ciudades)
-- ============================================================
DO $$
DECLARE
    v_ecu UUID := 'ec000000-0000-0000-0000-000000000000';
    v_pic UUID := 'ec000000-0000-0000-0000-000000000001';
    v_gua UUID := 'ec000000-0000-0000-0000-000000000002';
    v_gal UUID := 'ec000000-0000-0000-0000-000000000003';
    v_azu UUID := 'ec000000-0000-0000-0000-000000000004';
    v_imb UUID := 'ec000000-0000-0000-0000-000000000005';
    v_man UUID := 'ec000000-0000-0000-0000-000000000006';
    v_chi UUID := 'ec000000-0000-0000-0000-000000000007';
    v_elo UUID := 'ec000000-0000-0000-0000-000000000008';
    v_tun UUID := 'ec000000-0000-0000-0000-000000000009';
BEGIN
    -- País
    INSERT INTO locations (id, name, type, country_code) VALUES
        (v_ecu, 'Ecuador', 'Country', 'EC')
    ON CONFLICT (id) DO NOTHING;

    -- Provincias
    INSERT INTO locations (id, name, type, parent_id) VALUES
        (v_pic, 'Pichincha',      'State', v_ecu),
        (v_gua, 'Guayas',         'State', v_ecu),
        (v_gal, 'Galápagos',      'State', v_ecu),
        (v_azu, 'Azuay',          'State', v_ecu),
        (v_imb, 'Imbabura',       'State', v_ecu),
        (v_man, 'Manabí',         'State', v_ecu),
        (v_chi, 'Chimborazo',     'State', v_ecu),
        (v_elo, 'El Oro',         'State', v_ecu),
        (v_tun, 'Tungurahua',     'State', v_ecu)
    ON CONFLICT (id) DO NOTHING;

    -- Ciudades
    INSERT INTO locations (name, type, parent_id) VALUES
        ('Quito',           'City', v_pic),
        ('Cayambe',         'City', v_pic),
        ('Guayaquil',       'City', v_gua),
        ('Salinas',         'City', v_gua),
        ('Santa Cruz',      'City', v_gal),
        ('San Cristóbal',   'City', v_gal),
        ('Cuenca',          'City', v_azu),
        ('Gualaceo',        'City', v_azu),
        ('Ibarra',          'City', v_imb),
        ('Otavalo',         'City', v_imb),
        ('Manta',           'City', v_man),
        ('Montañita',       'City', v_man),
        ('Riobamba',        'City', v_chi),
        ('Alausí',          'City', v_chi),
        ('Machala',         'City', v_elo),
        ('Baños',           'City', v_tun),
        ('Ambato',          'City', v_tun)
    ON CONFLICT DO NOTHING;
END $$;

-- ============================================================
-- S6. CATEGORÍAS Y SUBCATEGORÍAS COMPLETAS
-- ============================================================
DO $$
DECLARE
    v_cult UUID := gen_random_uuid();
    v_ent  UUID := gen_random_uuid();
    v_nat  UUID := gen_random_uuid();
    v_aven UUID := gen_random_uuid();
    v_gas  UUID := gen_random_uuid();
    v_bien UUID := gen_random_uuid();
    v_trns UUID := gen_random_uuid();
BEGIN
    -- Categorías
    INSERT INTO category (id, slug, name, icon_url, sort_order) VALUES
        (v_cult, 'museos-arte-cultura',       'Museos, arte y cultura',       'museum.png',     1),
        (v_ent,  'entretenimiento-tickets',   'Entretenimiento y tickets',    'ticket.png',     2),
        (v_nat,  'naturaleza-vida-silvestre', 'Naturaleza y vida silvestre',  'leaf.png',       3),
        (v_aven, 'aventura-deporte',          'Aventura y deporte extremo',   'mountain.png',   4),
        (v_gas,  'gastronomia-bebidas',       'Gastronomía y bebidas',        'fork-knife.png', 5),
        (v_bien, 'bienestar-spa',             'Bienestar y spa',              'spa.png',        6),
        (v_trns, 'traslados-transporte',      'Traslados y transporte',       'car.png',        7);

    -- Subcategorías Museos/Cultura
    INSERT INTO subcategory (category_id, slug, name, sort_order) VALUES
        (v_cult, 'museos-culturales',         'Museos culturales',              1),
        (v_cult, 'monumentos-historicos',     'Monumentos y edificios históricos', 2),
        (v_cult, 'exposiciones-galerias',     'Exposiciones y galerías',        3),
        (v_cult, 'sitios-arqueologicos',      'Sitios arqueológicos',           4),
        (v_cult, 'iglesias-catedrales',       'Iglesias y catedrales',          5);

    -- Subcategorías Entretenimiento
    INSERT INTO subcategory (category_id, slug, name, sort_order) VALUES
        (v_ent, 'parques-tematicos',          'Parques temáticos',              1),
        (v_ent, 'conciertos-festivales',      'Conciertos y festivales',        2),
        (v_ent, 'deportes',                   'Eventos deportivos',             3),
        (v_ent, 'espectaculos-nocturnos',     'Espectáculos nocturnos',         4),
        (v_ent, 'escape-rooms',               'Escape rooms y juegos',          5);

    -- Subcategorías Naturaleza
    INSERT INTO subcategory (category_id, slug, name, sort_order) VALUES
        (v_nat, 'parques-nacionales',         'Parques nacionales y reservas',  1),
        (v_nat, 'avistamiento-aves',          'Avistamiento de aves',           2),
        (v_nat, 'snorkeling-buceo',           'Snorkeling y buceo',             3),
        (v_nat, 'playas-islas',               'Playas e islas',                 4),
        (v_nat, 'volcan-cascadas',            'Volcanes y cascadas',            5);

    -- Subcategorías Aventura
    INSERT INTO subcategory (category_id, slug, name, sort_order) VALUES
        (v_aven, 'senderismo-trekking',       'Senderismo y trekking',          1),
        (v_aven, 'ciclismo-mountain-bike',    'Ciclismo y mountain bike',       2),
        (v_aven, 'rafting-kayak',             'Rafting y kayak',                3),
        (v_aven, 'parapente',                 'Parapente y vuelos',             4),
        (v_aven, 'escalada',                  'Escalada y rappel',              5);

    -- Subcategorías Gastronomía
    INSERT INTO subcategory (category_id, slug, name, sort_order) VALUES
        (v_gas, 'tours-gastronomicos',        'Tours gastronómicos',            1),
        (v_gas, 'clases-cocina',              'Clases de cocina',               2),
        (v_gas, 'catas-vino-cafe',            'Catas de vino y café',           3),
        (v_gas, 'mercados-locales',           'Mercados y ferias locales',      4);

    -- Subcategorías Bienestar
    INSERT INTO subcategory (category_id, slug, name, sort_order) VALUES
        (v_bien, 'spa-masajes',               'Spa y masajes',                  1),
        (v_bien, 'yoga-meditacion',           'Yoga y meditación',              2),
        (v_bien, 'termas-aguas-termales',     'Termas y aguas termales',        3);

    -- Subcategorías Traslados
    INSERT INTO subcategory (category_id, slug, name, sort_order) VALUES
        (v_trns, 'traslado-aeropuerto',       'Traslado aeropuerto',            1),
        (v_trns, 'tours-en-bus',              'Tours en bus turístico',         2),
        (v_trns, 'tours-en-barco',            'Tours en barco y cruceros',      3),
        (v_trns, 'alquiler-vehiculos',        'Alquiler de vehículos',          4);

    -- Traducciones al inglés (categorías)
    INSERT INTO category_translation (category_id, language_id, name)
    SELECT c.id, 2, CASE c.slug
        WHEN 'museos-arte-cultura'       THEN 'Museums, Art & Culture'
        WHEN 'entretenimiento-tickets'   THEN 'Entertainment & Tickets'
        WHEN 'naturaleza-vida-silvestre' THEN 'Nature & Wildlife'
        WHEN 'aventura-deporte'          THEN 'Adventure & Extreme Sports'
        WHEN 'gastronomia-bebidas'       THEN 'Food & Drink'
        WHEN 'bienestar-spa'             THEN 'Wellness & Spa'
        WHEN 'traslados-transporte'      THEN 'Transfers & Transport'
    END
    FROM category c
    WHERE c.slug IN (
        'museos-arte-cultura','entretenimiento-tickets','naturaleza-vida-silvestre',
        'aventura-deporte','gastronomia-bebidas','bienestar-spa','traslados-transporte'
    )
    ON CONFLICT DO NOTHING;

    -- Traducciones al inglés (subcategorías) — muestra representativa
    INSERT INTO subcategory_translation (subcategory_id, language_id, name)
    SELECT s.id, 2, CASE s.slug
        WHEN 'museos-culturales'      THEN 'Cultural Museums'
        WHEN 'monumentos-historicos'  THEN 'Historical Monuments'
        WHEN 'exposiciones-galerias'  THEN 'Exhibitions & Galleries'
        WHEN 'sitios-arqueologicos'   THEN 'Archaeological Sites'
        WHEN 'iglesias-catedrales'    THEN 'Churches & Cathedrals'
        WHEN 'parques-tematicos'      THEN 'Theme Parks'
        WHEN 'conciertos-festivales'  THEN 'Concerts & Festivals'
        WHEN 'parques-nacionales'     THEN 'National Parks & Reserves'
        WHEN 'snorkeling-buceo'       THEN 'Snorkeling & Diving'
        WHEN 'senderismo-trekking'    THEN 'Hiking & Trekking'
        WHEN 'rafting-kayak'          THEN 'Rafting & Kayak'
        WHEN 'tours-gastronomicos'    THEN 'Gastronomic Tours'
        WHEN 'clases-cocina'          THEN 'Cooking Classes'
        WHEN 'spa-masajes'            THEN 'Spa & Massages'
        WHEN 'traslado-aeropuerto'    THEN 'Airport Transfer'
        ELSE s.name
    END
    FROM subcategory s
    ON CONFLICT DO NOTHING;
END $$;

-- ============================================================
-- S7. TAGS AMPLIADOS
-- ============================================================
INSERT INTO tag (name, slug) VALUES
    ('Aventura',        'aventura'),
    ('Cultura',         'cultura'),
    ('Romántico',       'romantico'),
    ('Nocturno',        'nocturno'),
    ('Fotografía',      'fotografia'),
    ('Lujo',            'lujo'),
    ('Familiar',        'familiar'),
    ('Ecológico',       'ecologico'),
    ('Gastronomía',     'gastronomia'),
    ('Historia',        'historia'),
    ('Relax',           'relax'),
    ('Acuático',        'acuatico'),
    ('Montaña',         'montana'),
    ('Patrimonio',      'patrimonio'),
    ('Accesible',       'accesible'),
    ('Solo Viajeros',   'solo-viajeros'),
    ('Grupos',          'grupos'),
    ('Pet Friendly',    'pet-friendly'),
    ('Guiado',          'guiado'),
    ('Autoguiado',      'autoguiado')
ON CONFLICT (slug) DO NOTHING;

-- ============================================================
-- S8. ITEMS DE INCLUSIÓN AMPLIADOS
-- ============================================================
-- language_id = 1 (Español), los ids de language son SERIAL
-- pero los insertamos después del seed de idiomas, así que usamos subquery
INSERT INTO inclusion_item (icon_slug, default_text, language_id)
SELECT icon, texto, (SELECT id FROM language WHERE iso_code = 'es')
FROM (VALUES
    ('bus',          'Transporte incluido'),
    ('utensils',     'Almuerzo incluido'),
    ('coffee',       'Snacks y bebidas'),
    ('wifi',         'WiFi a bordo'),
    ('hotel',        'Recogida en el hotel'),
    ('shield',       'Seguro de viaje'),
    ('headphones',   'Audioguía incluida'),
    ('camera',       'Sesión fotográfica'),
    ('umbrella',     'Equipo de protección'),
    ('life-ring',    'Equipo de seguridad acuática'),
    ('map',          'Mapa impreso'),
    ('ticket',       'Entrada incluida'),
    ('bottle-water', 'Agua embotellada'),
    ('user-guide',   'Guía certificado'),
    ('parking',      'Estacionamiento incluido'),
    ('first-aid',    'Botiquín de primeros auxilios'),
    ('bike',         'Bicicleta incluida'),
    ('wetsuit',      'Traje de neopreno')
) AS t(icon, texto)
ON CONFLICT DO NOTHING;

-- Versión en inglés de las inclusiones
INSERT INTO inclusion_item (icon_slug, default_text, language_id)
SELECT icon, texto, (SELECT id FROM language WHERE iso_code = 'en')
FROM (VALUES
    ('bus',          'Transportation included'),
    ('utensils',     'Lunch included'),
    ('coffee',       'Snacks and beverages'),
    ('wifi',         'On-board WiFi'),
    ('hotel',        'Hotel pickup'),
    ('shield',       'Travel insurance'),
    ('headphones',   'Audio guide included'),
    ('camera',       'Photo session'),
    ('umbrella',     'Protective gear'),
    ('life-ring',    'Water safety equipment'),
    ('map',          'Printed map'),
    ('ticket',       'Entry ticket included'),
    ('bottle-water', 'Bottled water'),
    ('user-guide',   'Certified guide'),
    ('parking',      'Parking included'),
    ('first-aid',    'First aid kit'),
    ('bike',         'Bicycle included'),
    ('wetsuit',      'Wetsuit provided')
) AS t(icon, texto)
ON CONFLICT DO NOTHING;

-- ============================================================
-- S9. PERFILES DE CLIENTES DEMO
-- ============================================================
DO $$
DECLARE v_quito UUID;
BEGIN
    SELECT id INTO v_quito FROM locations WHERE name = 'Quito' LIMIT 1;

    INSERT INTO client (user_id, first_name, last_name, phone, nationality, document_type, document_number, location_id, preferred_lang)
    SELECT u.id, first_name, last_name, phone, nationality, doc_type, doc_num, v_quito,
           (SELECT id FROM language WHERE iso_code = lang_code)
    FROM (VALUES
        ('30000000-0000-0000-0000-000000000003'::UUID, 'Yanick',   'Demo',     '+593999000001', 'Ecuatoriana', 'cedula',   '1710000001', 'es'),
        ('60000000-0000-0000-0000-000000000006'::UUID, 'María',    'García',   '+593999000002', 'Ecuatoriana', 'cedula',   '1720000002', 'es'),
        ('70000000-0000-0000-0000-000000000007'::UUID, 'Carlos',   'Méndez',   '+593999000003', 'Ecuatoriana', 'cedula',   '1730000003', 'es'),
        ('80000000-0000-0000-0000-000000000008'::UUID, 'Ana',      'Torres',   '+593999000004', 'Colombiana',  'passport', 'CC12345678', 'es'),
        ('90000000-0000-0000-0000-000000000009'::UUID, 'John',     'Smith',    '+1 555 000 001','American',    'passport', 'US987654321','en'),
        ('a0000000-0000-0000-0000-000000000010'::UUID, 'Sophie',   'Dupont',   '+33 6 00 00 01','Francesa',    'passport', 'FR123456789','fr')
    ) AS t(uid, first_name, last_name, phone, nationality, doc_type, doc_num, lang_code)
    JOIN users u ON u.id = t.uid
    ON CONFLICT (user_id) DO NOTHING;
END $$;

-- ============================================================
-- FIN DEL SCRIPT
-- ============================================================