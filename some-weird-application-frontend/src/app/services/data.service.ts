import { Injectable } from '@angular/core';
import { Data, WebsiteInfo } from '../models';
import { HttpClient, HttpParams } from '@angular/common/http';
import { of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  dataUrl = 'https://localhost:44384/api/data';
  domainUrl = 'https://localhost:44384/api/referred';
  siteListUrl = 'https://localhost:44384/api/WebSiteInfo';
  interestingSiteListUrl = 'https://localhost:44384/api/WebSiteInfo/interesting';

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
    return this.http.get<WebsiteInfo[]>(this.domainUrl, {params: param}).toPromise()
    .then(
      stat => {
        console.log(stat);
        let converted = new Array<any>();
        stat.forEach(elem => {
          converted.push(
            [elem.domain,
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

  getAllSites(): Promise<WebsiteInfo[]>{
    return this.http.get<WebsiteInfo[]>(this.siteListUrl).toPromise()
    .then(sites => sites)
    .catch(err => {
      console.log(err);
      return new Array<WebsiteInfo>();
    });
  }

  getInterestingSites(): Promise<WebsiteInfo[]>{
    return this.http.get<WebsiteInfo[]>(this.interestingSiteListUrl).toPromise()
    .then(sites => sites)
    .catch(err => {
      console.log(err);
      return new Array<WebsiteInfo>();
    });
  }
}
