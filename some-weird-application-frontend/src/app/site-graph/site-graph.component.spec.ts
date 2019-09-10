import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SiteGraphComponent } from './site-graph.component';

describe('SiteGraphComponent', () => {
  let component: SiteGraphComponent;
  let fixture: ComponentFixture<SiteGraphComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SiteGraphComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SiteGraphComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
