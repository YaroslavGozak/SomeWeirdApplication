import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { WebsiteInfo } from '../models';
import { DataService } from '../services/data.service';
import { faAngleDown, faAngleUp } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-site-list',
  templateUrl: './site-list.component.html',
  styleUrls: ['./site-list.component.scss']
})

export class SiteListComponent implements OnInit {
  private interestingSites: WebsiteInfo[];
  private allSites: WebsiteInfo[];

  @Output() url: EventEmitter<string> = new EventEmitter();

  expandInterestingSites = false;
  expandAllSites = true;

  faAngleDown = faAngleDown;
  faAngleUp = faAngleUp;

  allSitesStateIcon = faAngleUp;
  interestingSitesStateIcon = faAngleDown;

  constructor(private service: DataService) { }

  ngOnInit() {
    this.service.getAllSites().then(sites => this.allSites = sites);
    this.service.getInterestingSites().then(sites => this.interestingSites = sites);
  }

  siteClicked(url: string){
    console.log(url);
    this.url.emit(url);
  }

  toggleAllSites(){
    this.expandAllSites = !this.expandAllSites;
    this.allSitesStateIcon = this.expandAllSites ? faAngleUp : faAngleDown;
  }

  toggleInterestingSites(){
    this.expandInterestingSites = !this.expandInterestingSites;
    this.interestingSitesStateIcon = this.expandInterestingSites ? faAngleUp : faAngleDown;
  }
}