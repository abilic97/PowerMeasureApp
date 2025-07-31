import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DynamicoptionsComponent } from './dynamicoptions.component';

describe('DynamicoptionsComponent', () => {
  let component: DynamicoptionsComponent;
  let fixture: ComponentFixture<DynamicoptionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DynamicoptionsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DynamicoptionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
