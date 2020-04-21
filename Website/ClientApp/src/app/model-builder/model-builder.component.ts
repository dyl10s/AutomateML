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

  errorModal: boolean = false;
  errorText: string = "";
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

    //Validation
    if(this.trainModel.Title == null || this.trainModel.Title.length == 0){
      this.errorText = "Please enter a Model Name.";
      this.errorModal = true;
      return;
    }

    if(this.trainModel.ModelType == null || (![1, 2, 3].includes(this.trainModel.ModelType))){
      this.errorText = "Please enter a Model Type.";
      this.errorModal = true;
      return;
    }

    if(this.trainModel.LabelColumn == null || this.trainModel.LabelColumn.length == 0){
      this.errorText = "Please select an Output Column.";
      this.errorModal = true;
      return;
    }

    this.trainModel.Columns.forEach(x => {
      if(x.ColumnName === this.trainModel.LabelColumn){
        if(x.Type == 1){
          this.errorText = "The output column can't be marked ignore value.";
          this.errorModal = true;
          return;
        }

        //Binary
        if(this.trainModel.ModelType == 1){
          if(x.Type != 12){ //True/False
            this.errorText = "The output column must be marked 'True/False' for Binary Classification";
            this.errorModal = true;
            return;
          }
        }

        //Regression
        if(this.trainModel.ModelType == 3){
          if(x.Type != 9){ //Number
            this.errorText = "The output column must be marked 'Number' for Regression";
            this.errorModal = true;
            return;
          }
        }
      }
    });

    if(this.errorModal){
      return;
    }

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
          }else{
            this.errorModal = true;
            console.log(success.exception);
            this.errorText = `
              An unknown error has occured. Make sure there are no errors in your csv, and all data types are correctly selected. Blank values in CSVs
              can sometimes cause problems.
              `;
          }
        },
        error => {
          this.errorModal = true;
          this.errorText = "An unknown error has occured. Please try again.";
          this.modalShowing = false;
        });

    console.log(this.trainModel);
  }


  ModelTypeTooltip: string = `
  This is the type of model you want to train. <br/><br/>
  
  <strong>Binary Classification</strong> - This is for prediction of a true or false outcome. Your
  output column must be true / false. For example, whether a team will win or loose
  a basketball game based on half time score and timeouts used.<br/><br/>
      
  <strong>Multiclass Classification</strong> - This is when your output is multiple groups. For example, 
  the prediction of a flower type based on the dimensions of that flower.<br/><br/>
      
  <strong>Regression</strong> - This is when your output is a number. For example, 
  the price of a t-shirt given the brand, color and gender.
  `

  DataTypeTooltip: string = `
  This is the data type of the row in your csv file. <br/><br/>
  
  <strong>Number</strong> - This is any numerical value including decimals.<br/><br/>
      
  <strong>Text</strong> - This is any value that does not fit into any other data type.<br/><br/>
      
  <strong>True/False</strong> - This is for a true/false value. The text should be 'true' and 'false' in your data
  or '0' and '1'.<br/><br/>

  <strong>Ignore</strong> - Some times you have a value that you don't want to use for your model. You can mark this as ignore and
  it will not be used for training.
  `

  OutputTooltip: string = `
  This is what you want the computer to try and predict.
  Different model types require different output types. <br/><br/>

  <strong>Binary Classification</strong> - True/False<br/><br/>
      
  <strong>Multiclass Classification</strong> - Anything except ignore<br/><br/>
      
  <strong>Regression</strong> - Number
  `

  ModelDescriptionTooltip: string = `
  This is the description will be displayed on the models page. It is best
  to inform the users of what each field in the data is for and what your goal is.
  This will allow others to use your work without any hassle or guessing.
  `

  ModelNameTooltip: string = `
  This is the name of the model you are training. This is also how people will locate your model in the search.
  `

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


