import { Component, OnInit, Input } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-help-tip',
  templateUrl: './help-tip.component.html',
  styleUrls: ['./help-tip.component.css']
})
export class HelpTipComponent implements OnInit {

  @Input() Text: string;

  ngOnInit() {
    
  }

  constructor(private modalService: NgbModal) {}

  open(content) {
    this.modalService.open(content, {ariaLabelledBy: 'modal-basic-title'}).result.then((result) => {});
  }

}
