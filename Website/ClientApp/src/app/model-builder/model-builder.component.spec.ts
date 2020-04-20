import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { FormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ModelBuilderComponent } from './model-builder.component';
import { HelpTipComponent } from '../help-tip/help-tip.component';
import { JwtHelperService } from '@auth0/angular-jwt';

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

  it('modal should show when training starts', () => {
    component.startTraining();
    expect(component.modalShowing).toBeTruthy();
  });
});
