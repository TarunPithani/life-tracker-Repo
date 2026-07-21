import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

import { TodoForm } from '../../components/todo-form/todo-form';
import { CreateTodoRequest, UpdateTodoRequest } from '../../models/todo.model';
import { TodoService } from '../../services/todo.service';

@Component({
  selector: 'app-create-todo',
  imports: [RouterLink, MatButtonModule, TodoForm],
  templateUrl: './create-todo.html',
  styleUrl: './create-todo.scss',
})
export class CreateTodo {
  private readonly todoService = inject(TodoService);
  private readonly router = inject(Router);
  private readonly toastr = inject(ToastrService);

  protected onSubmit(payload: CreateTodoRequest | UpdateTodoRequest): void {
    const request: CreateTodoRequest = {
      title: payload.title,
      description: payload.description,
      dueDate: payload.dueDate,
      priority: payload.priority,
    };

    this.todoService.createTodo(request).subscribe({
      next: () => {
        this.toastr.success('Todo created');
        void this.router.navigate(['/todo']);
      },
    });
  }
}
