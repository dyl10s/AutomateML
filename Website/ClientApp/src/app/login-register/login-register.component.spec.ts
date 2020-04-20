import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { FormsModule, NgForm } from '@angular/forms';
import { LoginRegisterComponent } from './login-register.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { InjectionToken, inject } from '@angular/core';
import { getBaseUrl } from 'src/main';
import { JwtHelperService } from '@auth0/angular-jwt';

describe('LoginRegisterComponent', () => {
  let component: LoginRegisterComponent;
  let fixture: ComponentFixture<LoginRegisterComponent>;
  
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LoginRegisterComponent ],
      imports: [RouterTestingModule, FormsModule, HttpClientTestingModule],
      providers: [{ provide: 'BASE_URL', useValue: "test" }, {provide: JwtHelperService}]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginRegisterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('switch mode should change between login/regiser', () => {
    let startingMode = component.SigningUp;
    component.switchMode();
    expect(startingMode == component.SigningUp).toBeFalsy();
  });

  it('clear error message when login', () => {
    let form = new NgForm([], []);
    let startingMode = component.loginregister(form);
    expect(component.ErrorMessage == "").toBeTruthy();
  });
});
