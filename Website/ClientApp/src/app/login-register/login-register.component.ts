import { Component, OnInit, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { EventEmitter } from 'events';
import { GlobalState } from '../../services/global-state/global-state.service';
import { ReturnResult } from '../../objects/ReturnResult';

@Component({
  selector: 'app-login-register',
  templateUrl: './login-register.component.html',
  styleUrls: ['./login-register.component.css']
})
export class LoginRegisterComponent {

  SigningUp: boolean = false;
  Http: HttpClient;
  BaseUrl: string;
  headers = new HttpHeaders().set('Content-Type', 'application/json');
  Router: Router;
  ErrorMessage: string;
  IsLoading: boolean = false;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, router: Router, private state: GlobalState) {
    this.Http = http;
    this.BaseUrl = baseUrl;
    this.Router = router;
  }

  switchMode(){
    this.SigningUp = !this.SigningUp;
    this.ErrorMessage = "";
  }

  loginregister(form: NgForm) {
    let credentials = JSON.stringify(form.value);
    this.ErrorMessage = "";
    this.IsLoading = true;

    if (this.SigningUp) {

      this.Http.post(this.BaseUrl + "api/Account/Register", credentials, { headers: this.headers })
        .subscribe((success: ReturnResult<string>) => {
          if(success.success){
            localStorage.setItem("jwt", success.item)
            this.Router.navigate(["/"]);
            this.state.updateState();
          }else{
            this.ErrorMessage = success.errorMessage;
          }
          this.IsLoading = false;
        },
        error => {
          console.log(error);
          this.IsLoading = false;
          this.ErrorMessage = "An unknow error as occured. Please try again.";
        });

    } else {

      this.Http.post(this.BaseUrl + "api/Account/Login", credentials, { headers: this.headers })
        .subscribe((success: ReturnResult<string>) => {
          if(success.success){
            localStorage.setItem("jwt", success.item)
            this.Router.navigate(["/"]);
            this.state.updateState();
          }else{
            this.ErrorMessage = success.errorMessage;
          }
          this.IsLoading = false;
        },
        error => {
          console.log(error);
          this.ErrorMessage = "An unknow error as occured. Please try again.";
          this.IsLoading = false;
        });

    }
  }
}
