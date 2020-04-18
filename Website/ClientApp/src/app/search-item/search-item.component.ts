import { Component, OnInit, Input } from '@angular/core';
import { Model } from 'src/objects/Model';

@Component({
  selector: 'app-search-item',
  templateUrl: './search-item.component.html',
  styleUrls: ['./search-item.component.css']
})
export class SearchItemComponent implements OnInit {

  @Input() modelItem: Model;

  constructor() { }

  ngOnInit() {
  }

  openModelPage(){
    
  }

}
