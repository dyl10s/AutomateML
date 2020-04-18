import { Component, OnInit } from '@angular/core';
import { GlobalState } from '../../services/global-state/global-state.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit{
  isExpanded = false;
  isLoggedin = false;
  public username: string;

  constructor(private state: GlobalState) { }

  ngOnInit() {
    this.state.updateState();
    this.state.isLoggedIn.subscribe(value => this.isLoggedin = value);
    this.state.username.subscribe(value => this.username = value);
  }

  collapse() {
    this.isExpanded = false;
  }

  logout(){
    localStorage.removeItem("jwt");
    this.state.updateState();
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
