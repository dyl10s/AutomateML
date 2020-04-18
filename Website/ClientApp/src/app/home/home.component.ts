import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Model } from 'src/objects/Model';
import { ReturnResult } from 'src/objects/ReturnResult';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  public topTenModels: Model[];

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string){ 
    console.log(baseUrl);
    http.get<ReturnResult<Model[]>>(baseUrl + "api/Models/GetTopTen").subscribe(data => {
      this.topTenModels = data.item;
    });
  }
}
