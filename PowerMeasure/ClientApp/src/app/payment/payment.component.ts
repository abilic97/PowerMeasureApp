import { Component, Input, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { PaymentService } from '../service/payment.service';
import * as socketclient from "socket.io-client";
import Socket = socketclient.Socket;
import { DomSanitizer } from '@angular/platform-browser';
import { NewCardModel } from '../payment';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';



@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.css']
})
export class PaymentComponent implements OnInit {
  model = {} as NewCardModel;
  socket: Socket;
  Secure3DHTML: any;
  payRequestId: string;
  paymentResult: any;
  isLoading = false;
  messageToDisplay: string;
  @Input() billId: number;
  constructor(
    private paySvc: PaymentService,
    private sanitizer: DomSanitizer,
    public activeModal: NgbActiveModal
  ) { }

  ngOnInit(): void {
  }

  makePayment(form: NgForm) {
    if (form.submitted && form.valid) {
      this.model.billId = this.billId;
      console.log(this.model.billId)
      // form.value == this.model
      this.isLoading = true;
      this.paySvc.tokenizeCard(this.model).subscribe(
        res => {
          this.isLoading = false;
          if (res.completed) {
            res.response = JSON.parse(res.response);
            this.paymentResult = res;
           if(this.paymentResult.response.Status.ResultCode == "990017")
           {
            this.messageToDisplay = "Transaction successfull";
           }
           else {
            this.messageToDisplay = "Transaction unsuccessfull, try again"
           }
          } else {
            this.Secure3DHTML = this.sanitizer.bypassSecurityTrustResourceUrl(`data:text/html,${res.secure3DHtml}`);
            this.payRequestId = res.payRequestId;
            this.connectToSocket(res.payRequestId);
          }
        },
        err => {
          console.log(err);
        });
    }
  }

  connectToSocket(payId: string) {
    this.socket = this.paySvc.openSocketConnection(payId);
    this.socket.on('message', (e: any) => {
      console.log('connected to socket');
      // this.socketMessages = e;
    });
    this.socket.on('joined', (content: any) => {
      console.log(content);
      // this.socketMessages = content;
    });
    this.socket.on('complete', async (payload: any) => {
      this.Secure3DHTML = null;
      this.completeFollowUp();
    });
  }

  completeFollowUp() {
    this.paySvc.queryTransaction(this.payRequestId).toPromise()
      .then(async (res) => {
        this.paymentResult = res;
      }, async error => {
        console.error(error);
      });
  }
}
