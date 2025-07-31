import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class TokenInterceptorService implements HttpInterceptor {
  constructor(private inject: Injector) { }
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    let authService = this.inject.get(AuthService);
    let jwt = this.addTokenToHeader(req, authService.getToken());
    return next.handle(jwt).pipe(
      catchError(err => {
        return throwError(err);
      }
      )
    );
  }

  addTokenToHeader(req: HttpRequest<any>, token: any) {
    return req.clone({ headers: req.headers.set('Authorization', 'bearer ' + token) });
  }
}
