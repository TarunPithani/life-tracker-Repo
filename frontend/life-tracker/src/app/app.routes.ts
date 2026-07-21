import { Routes } from '@angular/router';
import { MainLayout } from './layouts/main-layout/main-layout';
import { AuthLayout } from './layouts/auth-layout/auth-layout';

export const routes: Routes = [
  {
    path: '',
    component: MainLayout,
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'todo',
      },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard').then((m) => m.Dashboard),
      },
      {
        path: 'todo',
        loadChildren: () =>
          import('./features/todo/todo.routes').then((m) => m.TODO_ROUTES),
      },
    ],
  },
  {
    path: 'auth',
    component: AuthLayout,
    children: [],
  },
  {
    path: '**',
    redirectTo: 'todo',
  },
];
