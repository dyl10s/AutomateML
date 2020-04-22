import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { FormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ModelBuilderComponent } from './model-builder.component';
import { HelpTipComponent } from '../help-tip/help-tip.component';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Console } from 'console';

describe('ModelBuilderComponent', () => {
  let component: ModelBuilderComponent;
  let fixture: ComponentFixture<ModelBuilderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [RouterTestingModule, FormsModule, HttpClientTestingModule],
      declarations: [ ModelBuilderComponent, HelpTipComponent ],
      providers: [{ provide: 'BASE_URL', useValue: "test" }, {provide: JwtHelperService}]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ModelBuilderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('modal should not show when training starts with no data', () => {
    component.startTraining();
    expect(component.modalShowing).toBeFalsy();
  });

  it('modal should show when training starts with valid data', () => {

    component.trainModel = {
      Title: "test",
      ModelType: 2,
      LabelColumn: "test",
      Columns: [
        {
          ColumnName: "test",
          ColumnIndex: 1,
          Type: 12
        }
      ],
      Separator: ',',
      Data: '',
      HasHeaders: false,
      Description: ""
    };

    component.startTraining();
    expect(component.modalShowing).toBeTruthy();
  });
});
