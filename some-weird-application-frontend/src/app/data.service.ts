import { Injectable } from '@angular/core';
import { Data, WebsiteStatistics } from './models';
import { HttpClient, HttpParams } from '@angular/common/http';
import {map} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  dataUrl = 'https://localhost:44384/api/data';
  domainUrl = 'https://localhost:44384/api/referred'

  constructor(private http: HttpClient) { }

  getData(): Promise<Data[]>{
    return this.http.get(this.dataUrl).toPromise()
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
      console.log("Error fetching from " + this.dataUrl);
      console.log(err);
      return new Array<Data>();
    });
  }

  getReferredDomains(url: string): Promise<any[]>{
    let param = new HttpParams().set('url', url);
    return this.http.get<WebsiteStatistics[]>(this.domainUrl, {params: param}).toPromise()
    .then(
      stat => {
        console.log(stat);
        let converted = new Array<any>();
        // let counter = 0;
        stat.forEach(elem => {
          // if(counter++ > 10)
          //   return;
          converted.push(
            [elem.url,
            elem.count]
          );
        });
        console.log(converted);
        let sum = 0;
        converted.forEach(element => {
          sum += element[1];
        });
        return converted;
      }
    )
    .catch(err => {
      console.log("Error fetching from " + this.domainUrl);
      console.log(err);
      return null;
    });
  }
}
