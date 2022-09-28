
-- we use postgis for spatial data.
create extension postgis;
-- we create a function/trigger to set updated at so we don't have to do it
-- in application code.
create or replace function set_updated_at()
    returns trigger as
$$
begin
    NEW.updated_utc = now();
    return NEW;
end;
$$ language plpgsql;

create or replace function trigger_updated_at(tablename regclass)
    returns void as
$$
begin
    execute format('CREATE TRIGGER set_updated_at
        AFTER UPDATE
        ON %s
        FOR EACH ROW
        WHEN (OLD is distinct from NEW)
    EXECUTE FUNCTION set_updated_at();', tablename);
end;
$$ language plpgsql;

create table restaurant (
    restaurant_id       uuid                primary key default gen_random_uuid(),
    name                text                not null check ( name <> '' ),
    description         text                not null check ( name <> '' ),
    created_utc         timestamptz         not null default now(),
    updated_utc         timestamptz         not null default now(),

    -- foreign keys
    administrator_id    uuid                not null,
    address_id          uuid                not null
);

create table address (
    address_id          uuid                primary key default gen_random_uuid(),
    text                text                not null check ( text <> '' ),
    location            geography(point)    not null
);

create table work_calendar (
    week_day                text            not null,
    opening_hour            time(0)         null,
    closing_hour            time(0)         null,
    
    -- foreign keys
    restaurant_id           uuid            not null,
    primary key (restaurant_id, week_day, opening_hour, closing_hour)
);

create table "user" (
    user_id             uuid                primary key default gen_random_uuid(),
    username            text                not null check ( username <> '' )
);

create table review (
    review_id           uuid                primary key default gen_random_uuid(),
    description         text                not null check ( description <> '' ),
    rating_taste        int                 not null check ( rating_taste between 0 and 10),
    rating_visual       int                 not null check ( rating_visual between 0 and 10),
    rating_texture      int                 not null check ( rating_texture between 0 and 10),
    -- didn't get to implement this in the code, however this is quite nice and is calculated on every insert to the database
    avg_rating          double precision    generated always as (( rating_taste + rating_visual + rating_texture) / 3) stored,
    created_utc         timestamptz         not null default now(),
    updated_utc         timestamptz         not null default now(),
    
    -- foreign keys
    user_id             uuid                not null,
    restaurant_id       uuid                not null
);

create table file_attachment (
     file_attachment_id  uuid               primary key default gen_random_uuid(),
     path                text               not null check ( path <> '' ),
     file_name           text               not null check ( file_name <> '' ),
     
     -- foreign keys
     review_id           uuid               null
);

select trigger_updated_at('"restaurant"');
select trigger_updated_at('"review"')

