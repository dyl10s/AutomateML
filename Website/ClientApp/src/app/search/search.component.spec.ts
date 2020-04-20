import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { FormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { SearchComponent } from './search.component';
import { SearchItemComponent } from '../search-item/search-item.component';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Model } from 'src/objects/Model';

describe('SearchComponent', () => {
  let component: SearchComponent;
  let fixture: ComponentFixture<SearchComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [RouterTestingModule, FormsModule, HttpClientTestingModule],
      declarations: [ SearchComponent, SearchItemComponent ],
      providers: [{ provide: 'BASE_URL', useValue: "test" }, {provide: JwtHelperService}]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('only search if text', () => {
    var resultModel = new Model();
    resultModel.modelId = 1;

    component.results = [resultModel];
    component.searchQuery = "";
    component.search();

    expect(component.results.length).toEqual(1);
  });

  it('clear on search', () => {
    var resultModel = new Model();
    resultModel.modelId = 1;

    component.results = [resultModel];
    component.searchQuery = "test";
    component.search();

    expect(component.results.length).toEqual(0);
  });
});
