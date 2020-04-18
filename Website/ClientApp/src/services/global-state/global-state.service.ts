import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class GlobalState {

    private usernameSource = new BehaviorSubject<string>(null);
    username = this.usernameSource.asObservable();

    private userIdSource = new BehaviorSubject<number>(null);
    userId = this.userIdSource.asObservable();

    private isLoggedInSource = new BehaviorSubject<boolean>(false);
    isLoggedIn = this.isLoggedInSource.asObservable();

    constructor(private jwtHelper: JwtHelperService) { }

    updateState(){
        let token = this.jwtHelper.tokenGetter()
        if(token && !this.jwtHelper.isTokenExpired(token)){
            this.isLoggedInSource.next(true);
            this.usernameSource.next(this.jwtHelper.decodeToken(token)["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"]);
        }else{
            this.isLoggedInSource.next(false);
            this.usernameSource.next(null);
        }
    }

}