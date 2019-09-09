import { Component, OnInit, Input } from '@angular/core';
import * as Highcharts from 'highcharts';
import { Data } from '../models';
import { DataService } from '../data.service';

declare var require: any;
let Boost = require('highcharts/modules/boost');
let noData = require('highcharts/modules/no-data-to-display');
let More = require('highcharts/highcharts-more');
let Wordcloud = require('highcharts/modules/wordcloud');

Boost(Highcharts);
noData(Highcharts);
More(Highcharts);
noData(Highcharts);
Wordcloud(Highcharts);

@Component({
  selector: 'app-donut-chart',
  templateUrl: './donut-chart.component.html',
  styleUrls: ['./donut-chart.component.scss']
})

export class DonutChartComponent implements OnInit {
  private options: any;
  private data: Data[];
  private submitted = false;

  public url: string = 'https://stackoverflow.com/questions/54706459/add-bootstrap-4-to-angular-6-or-angular-7-application';

  @Input() containerIdentifier: string = 'web-links-chart';

  constructor(private service: DataService) { }

  ngOnInit() {
    this.getReferredDomains();
  }

getReferredDomains(){
  this.service.getReferredDomains(this.url).then(data => {
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
    this.submitted = true;
    this.getReferredDomains();
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
              enabled: false
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