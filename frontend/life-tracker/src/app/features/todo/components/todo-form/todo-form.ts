import {
  Component,
  EventEmitter,
  inject,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
} from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

import {
  CreateTodoRequest,
  Todo,
  UpdateTodoRequest,
} from '../../models/todo.model';

@Component({
  selector: 'app-todo-form',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
  ],
  templateUrl: './todo-form.html',
  styleUrl: './todo-form.scss',
})
export class TodoForm implements OnChanges {
  @Input() todo?: Todo | null;
  @Input() submitLabel = 'Save';
  @Output() readonly formSubmit = new EventEmitter<
    CreateTodoRequest | UpdateTodoRequest
  >();

  private readonly fb = inject(FormBuilder);

  protected readonly priorities = [1, 2, 3, 4, 5];

  protected readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', [Validators.maxLength(2000)]],
    dueDate: ['', Validators.required],
    priority: this.fb.nonNullable.control(3, [
      Validators.required,
      Validators.min(1),
      Validators.max(5),
    ]),
  });

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['todo'] && this.todo) {
      this.form.patchValue({
        title: this.todo.title,
        description: this.todo.description ?? '',
        dueDate: this.todo.dueDate,
        priority: this.todo.priority,
      });
    }
  }

  protected onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.formSubmit.emit({
      title: value.title.trim(),
      description: value.description.trim() || null,
      dueDate: value.dueDate,
      priority: value.priority,
      ...(this.todo?.version ? { version: this.todo.version } : {}),
    });
  }
}
