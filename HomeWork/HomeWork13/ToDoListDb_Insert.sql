---todo_user

INSERT INTO public.todo_user (id, tg_user_name, registered_at, tg_userid)
VALUES ('a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11'::uuid, 'ivan_petrov', NOW(), 123456789);

INSERT INTO public.todo_user (id, tg_user_name, registered_at, tg_userid)
VALUES ('b1ffac1d-8e5f-4a3e-9c4b-2aa7bd490a22'::uuid, 'maria_ivanova', NOW(), 987654321);

INSERT INTO public.todo_user (id, tg_user_name, registered_at, tg_userid)
VALUES ('c2dde3ee-7f6a-4d5b-8c9d-3bb8ce500a33'::uuid, 'сергей_сидоров', NOW(), 555666777);

INSERT INTO public.todo_user (id, tg_user_name, registered_at, tg_userid)
VALUES ('d3eff4ff-8e7b-5e6c-9d0e-4cc9df610a44'::uuid, 'alex_tech', NOW(),111222333);

INSERT INTO public.todo_user (id, tg_user_name, registered_at, tg_userid)
VALUES ('e4f00550-9e8c-6f7d-0e1f-5dda0f720a55'::uuid, 'test_user_bot', NOW(), 111222334);


---todo_list
INSERT INTO public.todo_list (id, name, user_id, created_at) 
VALUES ('f5a01234-1a2b-3c4d-4e5f-6a7b8c9d0e1f', 'Личные задачи', 'a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11', NOW());

INSERT INTO public.todo_list (id, name, user_id, created_at)
VALUES ('f6b12345-2b3c-4d5e-5f6a-7b8c9d0e1f2a', 'Рабочие проекты', 'b1ffac1d-8e5f-4a3e-9c4b-2aa7bd490a22', NOW());

INSERT INTO public.todo_list (id, name, user_id, created_at)
VALUES ('f7c23456-3c4d-5e6f-6a7b-8c9d0e1f2a3b', 'Покупки', 'c2dde3ee-7f6a-4d5b-8c9d-3bb8ce500a33', NOW());

INSERT INTO public.todo_list (id, name, user_id, created_at)
VALUES ('f8d34567-4d5e-6f7a-7b8c-9d0e1f2a3b4c', 'Домашние дела', 'd3eff4ff-8e7b-5e6c-9d0e-4cc9df610a44', NOW());

INSERT INTO public.todo_list (id, name, user_id, created_at)
VALUES ('f9e45678-5e6f-7a8b-8c9d-0e1f2a3b4c5d', 'Разработка', 'e4f00550-9e8c-6f7d-0e1f-5dda0f720a55', NOW());


---todo_item
INSERT INTO public.todo_item (id, user_id, name, created_at, state, state_change_at, deadline, list_id)
VALUES (
    'a1b2c3d4-5e6f-7a8b-9c0d-1e2f3a4b5c6d',
    'a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11',
    'Записаться к врачу',
    '10:30:00+03',
    0,
    NULL,
    '2024-01-20 18:00:00+03',
    'f5a01234-1a2b-3c4d-4e5f-6a7b8c9d0e1f'
);

INSERT INTO public.todo_item (id, user_id, name, created_at, state, state_change_at, deadline, list_id)
VALUES (
    'b2c3d4e5-6f7a-8b9c-0d1e-2f3a4b5c6d7e',
    'b1ffac1d-8e5f-4a3e-9c4b-2aa7bd490a22',
    'Подготовить отчет за квартал',
    '09:15:00+03',
    1,
    '2024-01-16 17:00:00+03',
    '2024-01-25 23:59:00+03',
    'f6b12345-2b3c-4d5e-5f6a-7b8c9d0e1f2a'
);

INSERT INTO public.todo_item (id, user_id, name, created_at, state, state_change_at, deadline, list_id)
VALUES (
    'c3d4e5f6-7a8b-9c0d-1e2f-3a4b5c6d7e8f',
    'c2dde3ee-7f6a-4d5b-8c9d-3bb8ce500a33',
    'Купить продукты на неделю',
    '16:45:00+03',
    1,
    '2024-01-17 19:30:00+03',
    '2024-01-19 20:00:00+03',
    'f7c23456-3c4d-5e6f-6a7b-8c9d0e1f2a3b'
);

INSERT INTO public.todo_item (id, user_id, name, created_at, state, state_change_at, deadline, list_id)
VALUES (
    'd4e5f6a7-8b9c-0d1e-2f3a-4b5c6d7e8f9a',
    'd3eff4ff-8e7b-5e6c-9d0e-4cc9df610a44',
    'Сделать уборку в квартире',
    '11:20:00+03',
    0,
    NULL,
    '2024-01-21 22:00:00+03',
    'f8d34567-4d5e-6f7a-7b8c-9d0e1f2a3b4c'
);

INSERT INTO public.todo_item (id, user_id, name, created_at, state, state_change_at, deadline, list_id)
VALUES (
    'e5f6a7b8-9c0d-1e2f-3a4b-5c6d7e8f9a0b',
    'e4f00550-9e8c-6f7d-0e1f-5dda0f720a55',
    'Написать тесты для нового модуля',
    '14:10:00+03',
    1,
    '2024-01-18 16:45:00+03',
    '2024-01-30 23:59:00+03',
    'f9e45678-5e6f-7a8b-8c9d-0e1f2a3b4c5d'
);