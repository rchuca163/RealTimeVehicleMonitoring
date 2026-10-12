import { ComponentFixture, TestBed } from '@angular/core/testing';
import { VehicleMap } from './vehicle-map';

describe('VehicleMap', () => {
  let component: VehicleMap;
  let fixture: ComponentFixture<VehicleMap>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VehicleMap],
    }).compileComponents();

    fixture = TestBed.createComponent(VehicleMap);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
