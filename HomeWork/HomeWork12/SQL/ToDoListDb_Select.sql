/*
public async Task<int> CountActiveAsync(Guid userId, CancellationToken ct)
{
    return await Task.Run(() => _toDoItemList.Where(x => x.User.UserId == userId && x.State == ToDoItemState.Active).Count());
}
*/

SELECT count(*) FROM public.todo_item
WHERE id = 'd4e5f6a7-8b9c-0d1e-2f3a-4b5c6d7e8f9a' AND state = 0
---------------------
/*
public async Task<bool> ExistsByNameAsync(Guid userId, string name, CancellationToken ct)
{
	return await Task.Run(() => _toDoItemList.Where(x => x.User.UserId == userId && x.Name == name && x.State == ToDoItemState.Active).Count() > 0);
}
*/

SELECT count(*)	FROM public.todo_item
WHERE id = 'd4e5f6a7-8b9c-0d1e-2f3a-4b5c6d7e8f9a' AND name = 'Сделать уборку в квартире' AND state = 0		
------------------------	
/*
public async Task<ToDoItem?> GetAsync(Guid id, CancellationToken ct)
{
    return await Task.Run(() => _toDoItemList.Where(x => x.Id == id).FirstOrDefault());
}
*/
SELECT * FROM public.todo_item
WHERE id = 'd4e5f6a7-8b9c-0d1e-2f3a-4b5c6d7e8f9a'
-------------------------

/*
public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct)
{
    return await Task.Run(() => _toDoItemList.Where(x => x.User.UserId == userId && x.State == ToDoItemState.Active).ToList());
}
*/
SELECT * FROM public.todo_item
WHERE id = 'd4e5f6a7-8b9c-0d1e-2f3a-4b5c6d7e8f9a' AND state = 0
-------------------------

/*
public async Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken ct)
{
    return await Task.Run(() => _toDoItemList.Where(x => x.User.UserId == userId).ToList());
}
*/

SELECT * FROM public.todo_item
WHERE id = 'd4e5f6a7-8b9c-0d1e-2f3a-4b5c6d7e8f9a'