import { TestBed } from '@angular/core/testing';

import { GlobalState } from './global-state.service';

describe('GlobalState', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: GlobalState = TestBed.get(GlobalState);
    expect(service).toBeTruthy();
  });
});
