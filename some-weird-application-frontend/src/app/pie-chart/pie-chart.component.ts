import { Component, OnInit, Input, OnChanges, SimpleChanges, SimpleChange } from '@angular/core';
import * as Highcharts from 'highcharts';
import { Data } from '../models';
import { DataService } from '../services/data.service';

declare var require: any;
let Boost = require('highcharts/modules/boost');
let noData = require('highcharts/modules/no-data-to-display');
let More = require('highcharts/highcharts-more');

Boost(Highcharts);
noData(Highcharts);
More(Highcharts);
noData(Highcharts);

@Component({
  selector: 'app-pie-chart',
  templateUrl: './pie-chart.component.html',
  styleUrls: ['./pie-chart.component.scss']
})

export class PieChartComponent implements OnInit, OnChanges {

  private options: any;
  private data: Data[];

  @Input() url: string;

  @Input() containerIdentifier: string = 'web-links-chart';

  constructor(private service: DataService) { }

  ngOnInit() {
    if(this.url == null){
      this.url = 'https://stackoverflow.com/questions/54706459/add-bootstrap-4-to-angular-6-or-angular-7-application'
    }
    this.getReferredDomains(this.url);
  }

  ngOnChanges(changes: SimpleChanges): void {
    const currentUrl: SimpleChange = changes.url;
    if(currentUrl != null)
      this.getReferredDomains(currentUrl.currentValue);
  }

getReferredDomains(url: string){
  if(url == null)
    return;
  this.service.getReferredDomains(url).then(data => {
    this.data = data;
    if (data != null) {
      console.log(data);
      if (data.length > 15) {
        this.options = this.setColumnOptions(data);
        console.log('Column');
      }
      else {
        this.options = this.setPieOptions(data);
        console.log('Pie');
      }
      console.log(this.options);
      Highcharts.chart(this.containerIdentifier, this.options);
      console.log('--Options set--');
    }
  });
}

  onSubmit() { 
    console.log('Getting for: ' + this.url);
    this.getReferredDomains(this.url);
   }

  setPieOptions(data: any): any{
    var options: any = {
      chart: {
        type: 'pie',
        plotBackgroundColor: null,
        plotBorderWidth: null,
        plotShadow: false
      },
      title: {
        text: 'Referred domains'
      },
      tooltip: {
        pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
      },
      plotOptions:{
        pie: {
          allowPointSelect: true,
          cursor: 'pointer',
          dataLabels: {
            enabled: true,
            format: '<b>{point.name}</b>: {point.percentage:.1f} %'
          },
          showInLegend: false
        }
      },
      series: [
        {
          type: 'pie',
          name: 'Domains',
          colorByPoint: true,
          data: data
        }
      ]
    }
    return options;
  }

  setColumnOptions(data: any): any {
    var options: any = {
      chart: {
        type: 'column'
      },
      title: {
        text: 'Referred domains'
      },
      tooltip: {
        pointFormat: '{series.name}: <b>{point.y:.1f}%</b>'
      },
      xAxis: {
        type: 'category',
        labels: {
          rotation: -45,
          style: {
            fontSize: '13px',
            fontFamily: 'Verdana, sans-serif'
          }
        }
      },
      yAxis: {
        min: 0,
        title: {
          text: 'Site links percentage'
        }
      },
      series: [
        {
          type: 'column',
          name: 'Domains',
          colorByPoint: true,
          data: data
        }
      ],
      dataLabels: {
        enabled: true,
        rotation: -90,
        color: '#AAAAAA',
        align: 'right',
        format: '{point.y:.1f}', // one decimal
        y: 10, // 10 pixels down from the top
        style: {
          fontSize: '13px',
          fontFamily: 'Verdana, sans-serif'
        }
      }
    }
    
    return options;
  }
}