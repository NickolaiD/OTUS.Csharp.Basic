CREATE DATABASE "ToDoList"
    WITH
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'ru'
    LC_CTYPE = 'ru'
    LOCALE_PROVIDER = 'libc'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1
    IS_TEMPLATE = False;

---Users

CREATE TABLE IF NOT EXISTS public.todo_user
(
    id uuid NOT NULL,
    tg_user_name character varying(32) COLLATE pg_catalog."default" NOT NULL,
    registered_at timestamp with time zone NOT NULL,
    tg_userid bigint,
    chat_id bigint,
    CONSTRAINT todo_user_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.todo_user
    OWNER to postgres;


CREATE UNIQUE INDEX IF NOT EXISTS uq_todo_user_tg_userid
    ON public.todo_user USING btree
    (tg_userid ASC NULLS LAST)
    WITH (deduplicate_items=False)
    TABLESPACE pg_default;

----List

CREATE TABLE IF NOT EXISTS public.todo_list
(
    id uuid NOT NULL,
    name character varying(50) COLLATE pg_catalog."default" NOT NULL,
    user_id uuid NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT todo_list_pkey PRIMARY KEY (id),
    CONSTRAINT fk_todo_list_user FOREIGN KEY (user_id)
        REFERENCES public.todo_user (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.todo_list
    OWNER to postgres;


CREATE INDEX IF NOT EXISTS idx_todo_list_user_id
    ON public.todo_list USING btree
    (user_id ASC NULLS LAST)
    WITH (deduplicate_items=True)
    TABLESPACE pg_default;

---Item

CREATE TABLE IF NOT EXISTS public.todo_item
(
    id uuid NOT NULL,
    user_id uuid NOT NULL,
    name character varying(100) COLLATE pg_catalog."default" NOT NULL,
    created_at time with time zone NOT NULL,
    state integer NOT NULL,
    state_change_at time with time zone,
    deadline time with time zone NOT NULL,
    list_id uuid,
    CONSTRAINT todo_item_pkey PRIMARY KEY (id),
    CONSTRAINT fk_todo_item_list FOREIGN KEY (list_id)
        REFERENCES public.todo_list (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT fk_todo_item_user FOREIGN KEY (user_id)
        REFERENCES public.todo_user (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.todo_item
    OWNER to postgres;

CREATE INDEX IF NOT EXISTS idx_todo_item_list_id
    ON public.todo_item USING btree
    (list_id ASC NULLS LAST)
    WITH (deduplicate_items=True)
    TABLESPACE pg_default;


CREATE INDEX IF NOT EXISTS idx_todo_item_user_id
    ON public.todo_item USING btree
    (user_id ASC NULLS LAST)
    WITH (deduplicate_items=True)
    TABLESPACE pg_default;
	
---Notification

CREATE TABLE IF NOT EXISTS public.notification
(
    id uuid NOT NULL,
    user_id uuid NOT NULL,
    type character varying(100) COLLATE pg_catalog."default" NOT NULL,
    text character varying(1000) COLLATE pg_catalog."default" NOT NULL,
    scheduled_at timestamp without time zone,
    is_notified boolean NOT NULL,
    notified_at timestamp without time zone,
    CONSTRAINT notification_pkey PRIMARY KEY (id),
    CONSTRAINT fk_notification_user FOREIGN KEY (user_id)
        REFERENCES public.todo_user (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.notification
    OWNER to postgres;

CREATE INDEX IF NOT EXISTS idx_notification_user_id
    ON public.notification USING btree
    (user_id ASC NULLS LAST)
    WITH (deduplicate_items=True)
    TABLESPACE pg_default;
	
CREATE INDEX IF NOT EXISTS idx_notification_type
    ON public.notification USING btree
    (type COLLATE pg_catalog."default" ASC NULLS LAST)
    WITH (deduplicate_items=True)
    TABLESPACE pg_default;



	