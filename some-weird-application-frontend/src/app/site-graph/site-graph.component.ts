import { Component } from '@angular/core';


@Component({
  selector: 'app-site-graph',
  templateUrl: './site-graph.component.html',
  styleUrls: ['./site-graph.component.scss']
})

export class SiteGraphComponent {
  url: string = null;

  onUrlChanged(event){
    this.url = event;
  }
}