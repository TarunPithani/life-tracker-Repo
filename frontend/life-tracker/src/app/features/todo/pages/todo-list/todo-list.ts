import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import Swal from 'sweetalert2';

import { TodoCard } from '../../components/todo-card/todo-card';
import { TodoFilterComponent } from '../../components/todo-filter/todo-filter';
import { TodoFilter, TodoSummary } from '../../models/todo.model';
import { TodoService } from '../../services/todo.service';

@Component({
  selector: 'app-todo-list',
  imports: [RouterLink, MatButtonModule, MatIconModule, TodoCard, TodoFilterComponent],
  templateUrl: './todo-list.html',
  styleUrl: './todo-list.scss',
})
export class TodoList implements OnInit {
  private readonly todoService = inject(TodoService);

  private readonly todos = signal<TodoSummary[]>([]);
  private readonly search = signal('');

  protected readonly filteredTodos = computed(() => {
    const query = this.search().trim().toLowerCase();
    if (!query) {
      return this.todos();
    }

    return this.todos().filter((todo) =>
      todo.title.toLowerCase().includes(query),
    );
  });

  protected readonly errorMessage = signal<string | null>(null);

  private currentStatus: TodoFilter['status'] = 'all';

  ngOnInit(): void {
    this.loadTodos();
  }

  protected onFilterChange(filter: TodoFilter): void {
    this.search.set(filter.search ?? '');

    if (filter.status !== this.currentStatus) {
      this.currentStatus = filter.status ?? 'all';
      this.loadTodos();
    }
  }

  protected onComplete(id: number): void {
    this.todoService.completeTodo(id).subscribe({
      next: () => this.loadTodos(),
    });
  }

  protected async onDelete(id: number): Promise<void> {
    const result = await Swal.fire({
      title: 'Delete todo?',
      text: 'This action cannot be undone.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Delete',
      confirmButtonColor: '#d32f2f',
    });

    if (!result.isConfirmed) {
      return;
    }

    this.todoService.deleteTodo(id).subscribe({
      next: () => this.loadTodos(),
    });
  }

  private loadTodos(): void {
    this.errorMessage.set(null);
    this.todoService.getTodos(this.currentStatus).subscribe({
      next: (todos) => this.todos.set(todos),
      error: () => this.errorMessage.set('Failed to load todos.'),
    });
  }
}
