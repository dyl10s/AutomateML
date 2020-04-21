import { Component, OnInit, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ReturnResult } from '../../objects/ReturnResult';
import { Model } from '../../objects/Model';
import { ModelField } from 'src/objects/ModelField';
import { stringify } from 'querystring';
import { GlobalState } from 'src/services/global-state/global-state.service';

@Component({
  selector: 'app-model-page',
  templateUrl: './model-page.component.html',
  styleUrls: ['./model-page.component.css']
})
export class ModelPageComponent {

  predictFormError: string = null;
  predictionError: string = "";
  makingPrediction: boolean = false;
  votedForModel: boolean = false;
  modelVotes: number = 0;
  modelId: number;
  modelFields: ModelField[];
  currentModel: Model;
  lastPrediction: any;
  isLoggedin: boolean = false;
  headers = new HttpHeaders().set('Content-Type', 'application/json');
  
  constructor(private route: ActivatedRoute, public Http: HttpClient, @Inject('BASE_URL') public BaseUrl: string, public state: GlobalState) { 
    
    this.modelId = parseInt(this.route.snapshot.params["id"]);
    this.state.isLoggedIn.subscribe(value => this.isLoggedin = value);

    this.Http.get<ReturnResult<Model>>(this.BaseUrl + "api/Models/GetModel?modelId=" + this.modelId)
    .subscribe((success: ReturnResult<Model>) => {
      if(success.success){
        
        this.currentModel = success.item;

      }else{
        console.log(success.exception);
      }
    });

    this.Http.get<ReturnResult<ModelField[]>>(this.BaseUrl + "api/ModelField/GetModelFields?modelId=" + this.modelId).subscribe(
      ((success: ReturnResult<ModelField[]>) => {
        this.modelFields = success.item;
      })
    );

    if(this.isLoggedin){
      this.Http.get<ReturnResult<number>>(this.BaseUrl + "api/Models/GetModelVotes?modelId=" + this.modelId).subscribe(
        ((success: ReturnResult<number>) => {
          this.modelVotes = success.item;
        })
      );
  
      this.Http.get<ReturnResult<boolean>>(this.BaseUrl + "api/Models/HasVotedForModel?modelId=" + this.modelId).subscribe(
        ((success: ReturnResult<boolean>) => {
          this.votedForModel = success.item;
        })
      );
    }
  }

  GetInputFields() : ModelField[]{
    let inputs: ModelField[] = [];

    if(this.modelFields != null){
      this.modelFields.forEach(element => {
        if(!element.isOutput && element.dataTypeId != 1){
          inputs.push(element);
        }
      });
    }

    return inputs;
  }

  GetOutputField(): string{
    let results = "";

    if(this.modelFields != null){
      this.modelFields.forEach(element => {
        if(element.isOutput){
          results = element.name;
        }
      });
    }

    return results;
  }

  ToggleVote(){
    this.Http.post<ReturnResult<boolean>>(this.BaseUrl + "api/Models/VoteForModel?modelId=" + this.modelId, null, { headers: this.headers }).subscribe(
      ((success: ReturnResult<boolean>) => {
        if(success.success){
          if(success.item == true){
            this.modelVotes += 1;
            this.votedForModel = true;
          }else{
            this.modelVotes -= 1;
            this.votedForModel = false;
          }
        }
      })
    );
  }

  PredictModel(){
    this.makingPrediction = true;
    this.lastPrediction = null;
    this.predictionError = null;
    this.predictFormError = null;

    let predictionData: string = "";
    this.modelFields.forEach(e => {
      if(e.isOutput){
        predictionData += '"", ';
      }else if(e.dataTypeId != 1){
        if(!e.currentValue || e.currentValue.length == 0){
          this.predictFormError = "Please enter all the fields.";
          this.makingPrediction = false;
        }
        predictionData += '"' + e.currentValue + '", '
      }
    })

    if(!this.makingPrediction){
      return;
    }

    predictionData = predictionData.substring(0, predictionData.length - 2);

    let predictionInfo : PredictInput = new PredictInput();
    predictionInfo.ModelId = this.modelId;
    predictionInfo.CsvData = predictionData;
    

    this.Http.post<ReturnResult<any>>(this.BaseUrl + "api/Models/GetPrediction", predictionInfo, { headers: this.headers }).subscribe(
      (success: ReturnResult<any>) => {
        if(success.success){
          this.lastPrediction = success.item;
        }else{
          this.lastPrediction = null;
          this.predictionError = success.errorMessage;
        }
        
        this.makingPrediction = false;
      },
      (error) => {
        this.makingPrediction = false;
        this.lastPrediction = null;
        this.predictionError = "There was an error making the prediction";
      });
  }

}

class PredictInput {
  public ModelId: number;
  public CsvData: string = "";
}


