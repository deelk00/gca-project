import { TestBed } from '@angular/core/testing';

import { GraphQLService } from './graph-q-l.service';

describe('GraphQlService', () => {
  let service: GraphQLService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GraphQLService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
