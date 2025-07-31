import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DailyCostComponent } from './daily-cost.component';

describe('DailyCostComponent', () => {
  let component: DailyCostComponent;
  let fixture: ComponentFixture<DailyCostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DailyCostComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DailyCostComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
