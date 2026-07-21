import { Component, inject, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

import { TodoForm } from '../../components/todo-form/todo-form';
import { CreateTodoRequest, Todo, UpdateTodoRequest } from '../../models/todo.model';
import { TodoService } from '../../services/todo.service';

@Component({
  selector: 'app-edit-todo',
  imports: [RouterLink, MatButtonModule, TodoForm],
  templateUrl: './edit-todo.html',
  styleUrl: './edit-todo.scss',
})
export class EditTodo implements OnInit {
  private readonly todoService = inject(TodoService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly toastr = inject(ToastrService);

  protected readonly todo = signal<Todo | null>(null);
  protected readonly errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (!Number.isFinite(id)) {
      this.errorMessage.set('Todo not found.');
      return;
    }

    this.todoService.getTodoById(id).subscribe({
      next: (todo) => this.todo.set(todo),
      error: () => this.errorMessage.set('Failed to load todo.'),
    });
  }

  protected onSubmit(payload: CreateTodoRequest | UpdateTodoRequest): void {
    const current = this.todo();
    if (!current) {
      return;
    }

    const request: UpdateTodoRequest = {
      title: payload.title,
      description: payload.description,
      dueDate: payload.dueDate,
      priority: payload.priority,
      version: 'version' in payload ? payload.version : current.version,
    };

    this.todoService.updateTodo(current.id, request).subscribe({
      next: () => {
        this.toastr.success('Todo updated');
        void this.router.navigate(['/todo']);
      },
    });
  }
}
