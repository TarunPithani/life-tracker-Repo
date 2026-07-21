import { Component, DestroyRef, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { debounceTime, distinctUntilChanged } from 'rxjs';

import { TodoFilter, TodoFilterStatus } from '../../models/todo.model';

@Component({
  selector: 'app-todo-filter',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
  ],
  templateUrl: './todo-filter.html',
  styleUrl: './todo-filter.scss',
})
export class TodoFilterComponent implements OnInit {
  @Output() readonly filterChange = new EventEmitter<TodoFilter>();

  private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly form = this.fb.nonNullable.group({
    search: [''],
    status: this.fb.nonNullable.control<TodoFilterStatus>('all'),
  });

  ngOnInit(): void {
    this.form.controls.search.valueChanges
      .pipe(debounceTime(250), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.emitFilter());

    this.form.controls.status.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.emitFilter());
  }

  protected clearSearch(): void {
    this.form.controls.search.setValue('');
  }

  private emitFilter(): void {
    const { search, status } = this.form.getRawValue();
    this.filterChange.emit({
      search: search.trim() || undefined,
      status,
    });
  }
}
