import { DatePipe } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';

import { TodoSummary } from '../../models/todo.model';

@Component({
  selector: 'app-todo-card',
  imports: [
    DatePipe,
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatChipsModule,
    MatIconModule,
  ],
  templateUrl: './todo-card.html',
  styleUrl: './todo-card.scss',
})
export class TodoCard {
  @Input({ required: true }) todo!: TodoSummary;
  @Output() readonly delete = new EventEmitter<number>();
  @Output() readonly complete = new EventEmitter<number>();
}
