import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ReturnResult } from 'src/objects/ReturnResult';
import { Model } from 'src/objects/Model';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})

export class SearchComponent implements OnInit {

  hasSearched = false;
  results: Model[] = [];
  searchQuery: string = "";
  constructor(private http: HttpClient, @Inject('BASE_URL') private BaseUrl: string) { }

  ngOnInit() {
  }

  search(){
    if(this.searchQuery.trim() != ""){
      this.results = [];
      this.hasSearched = false;

      this.http.get<ReturnResult<Model[]>>(this.BaseUrl + "api/Models/SearchModel?query=" + this.searchQuery).subscribe((data: ReturnResult<Model[]>) => {
        data.item.forEach(x => {
          this.results.push(x);
        });
        this.hasSearched = true;
      });
    }
  }

}
