import { Component, OnInit } from '@angular/core';
import * as Highcharts from 'highcharts';
import { Data } from './../models';
import { DataService } from './../data.service';

declare var require: any;
let Boost = require('highcharts/modules/boost');
let noData = require('highcharts/modules/no-data-to-display');
let More = require('highcharts/highcharts-more');

Boost(Highcharts);
noData(Highcharts);
More(Highcharts);
noData(Highcharts);

@Component({
  selector: 'app-output-graph',
  templateUrl: './output-graph.component.html',
  styleUrls: ['./output-graph.component.scss']
})

export class OutputGraphComponent implements OnInit {
  private data: Data[];

  constructor(private service: DataService){}

  ngOnInit(){
    this.service.getData().then(data => 
    {
      this.data = data;
      console.log("----OutputGraphComponent----");
      var options = this.setOptions(data);
      Highcharts.chart('container', options);
    });
  }

  setOptions(data: Data[]): any{
    if(data[0] == null || data[1] == null){
      data[0] = new Data(); data[1] = new Data();
    }
    var options: any = {
      chart: {
        type: 'scatter',
        height: 700
      },
      title: {
        text: 'Sample Scatter Plot'
      },
      credits: {
        enabled: false
      },
      tooltip: {
        formatter: function() {
          return 'x: ' + Highcharts.dateFormat('%e %b %y %H:%M:%S', this.x) +
            'y: ' + this.y.toFixed(2);
        }
      },
      xAxis: {
        type: 'datetime',
        labels: {
          formatter: function() {
            return Highcharts.dateFormat('%e %b %y', this.value);
          }
        }
      },
      series: [
        {
          name: 'Normal',
          turboThreshold: 500000,
          data: data[0].data
        },
        {
          name: 'Abnormal',
          turboThreshold: 500000,
          data: data[1].data
        }
      ]
    }
    return options;
  }
}