export type TodoFilterStatus = 'all' | 'pending' | 'completed';

export interface TodoSummary {
  id: number;
  title: string;
  isCompleted: boolean;
  dueDate: string;
  priority: number;
}

export interface Todo extends TodoSummary {
  description?: string | null;
  createdOn?: string | null;
  modifiedOn?: string | null;
  version?: string | null;
}

export interface TodoFilter {
  search?: string;
  status?: TodoFilterStatus;
}

export interface CreateTodoRequest {
  title: string;
  description?: string | null;
  dueDate: string;
  priority: number;
}

export interface UpdateTodoRequest {
  title: string;
  description?: string | null;
  dueDate: string;
  priority: number;
  version?: string | null;
}
