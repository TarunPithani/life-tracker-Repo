import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ApiService } from '../../../core/services/api.service';
import {
  CreateTodoRequest,
  Todo,
  TodoFilterStatus,
  TodoSummary,
  UpdateTodoRequest,
} from '../models/todo.model';

@Injectable({
  providedIn: 'root',
})
export class TodoService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'todos';

  getTodos(status: TodoFilterStatus = 'all'): Observable<TodoSummary[]> {
    const path =
      status === 'completed'
        ? `${this.endpoint}/completed`
        : status === 'pending'
          ? `${this.endpoint}/pending`
          : this.endpoint;

    return this.api.get<TodoSummary[]>(path);
  }

  getTodoById(id: number): Observable<Todo> {
    return this.api.get<Todo>(`${this.endpoint}/${id}`);
  }

  createTodo(payload: CreateTodoRequest): Observable<Todo> {
    return this.api.post<Todo>(this.endpoint, payload);
  }

  updateTodo(id: number, payload: UpdateTodoRequest): Observable<Todo> {
    return this.api.put<Todo>(`${this.endpoint}/${id}`, payload);
  }

  deleteTodo(id: number): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }

  completeTodo(id: number): Observable<Todo> {
    return this.api.patch<Todo>(`${this.endpoint}/${id}/complete`, {});
  }
}
