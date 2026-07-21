import { Routes } from '@angular/router';

export const TODO_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/todo-list/todo-list').then((m) => m.TodoList),
  },
  {
    path: 'create',
    loadComponent: () =>
      import('./pages/create-todo/create-todo').then((m) => m.CreateTodo),
  },
  {
    path: 'edit/:id',
    loadComponent: () =>
      import('./pages/edit-todo/edit-todo').then((m) => m.EditTodo),
  },
];
