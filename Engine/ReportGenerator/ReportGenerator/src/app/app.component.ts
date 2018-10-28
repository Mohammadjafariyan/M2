import { Component } from '@angular/core';
import {DataComponent} from "./query-generator/data/data.component";

@Component({
  moduleId:'AppComponent',
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'ReportGenerator';

  static ShowMsg(s: string, err: string, errmsg: string) {
    alert(errmsg);
  }
}
