import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { JwtModule } from "@auth0/angular-jwt";

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { FooterComponent } from './footer/footer.component';
import { SearchComponent } from './search/search.component';
import { LoginRegisterComponent } from './login-register/login-register.component';
import { ModelBuilderComponent } from './model-builder/model-builder.component';
import { AuthguardService } from '../services/authguard/authguard.service';
import { SearchItemComponent } from './search-item/search-item.component';
import { ModelPageComponent } from './model-page/model-page.component';

export function tokenGetter() {
  return localStorage.getItem("jwt");
}

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    FooterComponent,
    SearchComponent,
    LoginRegisterComponent,
    ModelBuilderComponent,
    SearchItemComponent,
    ModelPageComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'app' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'search', component: SearchComponent, pathMatch: 'full' },
      { path: 'model/:id', component: ModelPageComponent, pathMatch: 'full' },
      { path: 'login', component: LoginRegisterComponent, pathMatch: 'full' },
      { path: 'builder', component: ModelBuilderComponent, pathMatch: 'full', canActivate: [AuthguardService] }
    ], { scrollPositionRestoration: 'top' }),
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        whitelistedDomains: ["localhost:5001", "automate.ml", "www.automate.ml"]
      }
    })
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
