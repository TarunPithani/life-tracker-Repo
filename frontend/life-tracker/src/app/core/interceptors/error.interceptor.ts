import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (request, next) => {
  const toastr = inject(ToastrService);

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      toastr.error(getErrorMessage(error), 'Request failed');
      return throwError(() => error);
    }),
  );
};

function getErrorMessage(error: HttpErrorResponse): string {
  if (error.status === 0) {
    return 'Unable to connect to the server.';
  }

  if (typeof error.error === 'string' && error.error.trim()) {
    return error.error;
  }

  if (error.error?.message) {
    return error.error.message;
  }

  if (error.message) {
    return error.message;
  }

  return `Request failed with status ${error.status}.`;
}
