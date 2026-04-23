export interface OperatorLoginRequest {
  email: string;
  password: string;
}

export interface OperatorSignupRequest {
  operatorName: string;
  email: string;
  phoneNumber: string;
  licenseNumber: string;
  address: string;
  password: string;
}

export interface OperatorAuthResponse {
  token: string;
  operator: OperatorDto;
  message: string;
}

export interface OperatorDto {
  id: number;
  operatorName: string;
  email: string;
  phoneNumber: string;
  licenseNumber: string;
  address: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface BusDto {
  id: number;
  registrationNumber: string;
  operatorId: number;
  routeId: number;
  sourceLocationId: number;
  destinationLocationId: number;
  seatingCapacity?: number;
  price: number;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateBusDto {
  registrationNumber: string;
  operatorId: number;
  routeId: number;
  sourceLocationId: number;
  destinationLocationId: number;
  seatingCapacity?: number;
  price: number;
}

export interface UpdateBusDto {
  registrationNumber: string;
  routeId: number;
  sourceLocationId: number;
  destinationLocationId: number;
  seatingCapacity?: number;
  price: number;
  isActive: boolean;
}

export interface LocationDto {
  id: number;
  streetAddress: string;
  districtId: number;
  city: string;
  stateId: number;
  countryId: number;
  postalCode: string;
  latitude?: number;
  longitude?: number;
  operatorId?: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateLocationDto {
  streetAddress: string;
  districtId: number;
  city: string;
  stateId: number;
  countryId: number;
  postalCode: string;
  latitude?: number;
  longitude?: number;
  operatorId?: number;
}

export interface UpdateLocationDto {
  streetAddress: string;
  districtId: number;
  city: string;
  stateId: number;
  countryId: number;
  postalCode: string;
  latitude?: number;
  longitude?: number;
  operatorId?: number;
}
