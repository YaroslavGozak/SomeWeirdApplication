import { Injectable } from '@angular/core';
import { Data } from './models';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  configUrl = 'https://localhost:44384/api/data';

  constructor(private http: HttpClient) { }

  getData(): Promise<Data[]>{
    return this.http.get(this.configUrl).toPromise()
    .then(p => {
      var temp =  p as Data[];
      temp.forEach(element => {
        element.data.forEach(data => {
          data[0] = new Date(data[0]).getTime();
        });
      })
      return temp;
    })
    .catch(err => {
      console.log("Error fetching from " + this.configUrl);
      console.log(err);
      return new Array<Data>();
    });
  }
}
