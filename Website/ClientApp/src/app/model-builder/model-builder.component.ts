import { Component, OnInit, NgZone, Inject } from '@angular/core';
import { Papa } from 'ngx-papaparse';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { ReturnResult } from '../../objects/ReturnResult';
import { Model } from '../../objects/Model';

@Component({
  selector: 'app-model-builder',
  templateUrl: './model-builder.component.html',
  styleUrls: ['./model-builder.component.css']
})

export class ModelBuilderComponent implements OnInit {

  trainModel: TrainInput = new TrainInput();
  headers = new HttpHeaders().set('Content-Type', 'application/json');
  modalShowing: boolean = false;
  trainComplete: boolean = false;
  resultsModel: Model; 

  constructor(private ngZone: NgZone, private papa: Papa, @Inject('BASE_URL') public BaseUrl: string, public Http: HttpClient) { }

  ngOnInit() { }

  startFileUpload() {
    document.getElementById("fileUpload").click();
  }

  handleFileUpload(event) {

    //Load csv file and get types
    let options = {
      complete: (results, file) => this.ngZone.run(() => {

          this.trainModel.Columns = [];

          results.meta.fields.forEach((e: string, i: number) => {
            
            //Add header to model
            let colInfo = new ColumInformation();
            colInfo.ColumnIndex = i;
            colInfo.ColumnName = e;

            if(typeof(results.data[i][e]) === "number"){
              colInfo.Type = HeaderTypes.number;
            }else if(typeof(results.data[i][e]) === "string"){
              colInfo.Type = HeaderTypes.text;
            }else if(typeof(results.data[i][e]) === "boolean"){
              colInfo.Type = HeaderTypes.trueFalse;
            }else{
              colInfo.Type = HeaderTypes.text;
            }
            
            this.trainModel.Columns.push(colInfo);
          });
      }),
      header: true,
      dynamicTyping: true
    };

    this.papa.parse(event.target.files[0], options)

    //Get raw file data as string
    let reader = new FileReader();
    reader.onload = (e: any) => this.ngZone.run(() => {
      this.trainModel.Data = e.target.result;
    });
    reader.readAsText(event.target.files[0]);
  }

  startTraining(){
    this.modalShowing = true;

    this.trainModel.HasHeaders = true;
    this.trainModel.Separator = ',';

    this.Http.post<ReturnResult<Model>>(this.BaseUrl + "api/Models/StartTraining", this.trainModel, { headers: this.headers })
        .subscribe((success: ReturnResult<Model>) => {
          console.log(success);
          this.modalShowing = false;
          if(success.success){
            this.trainComplete = true;
            this.resultsModel = success.item;
          }
        },
        error => {
          console.log(error);
          this.modalShowing = false;
        });

    console.log(this.trainModel);
  }


}

class TrainInput {
  public Title: string = "";
  public LabelColumn: string = "";
  public Separator: string = "";
  public Columns: ColumInformation[];
  public Data: string = "";
  public Description: string = "";
  public HasHeaders: boolean = false;
  public ModelType: number;
}

class ColumInformation{
  public ColumnName: string;
  public ColumnIndex: number;
  public Type: HeaderTypes;
}

enum HeaderTypes{
    number = 9,
    text = 11,
    trueFalse = 12,
    Ignore = 1
}


