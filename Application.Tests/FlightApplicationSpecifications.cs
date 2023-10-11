

using System.Linq;
using FluentAssertions;
using Data;
using Domain;
using Microsoft.EntityFrameworkCore;
using Application.Tests;

namespace Application.Tests

{
    public class FlightApplicationSpecifications
    {

        readonly Entities entities = new Entities(new DbContextOptionsBuilder<Entities>()
                .UseInMemoryDatabase("Flights")
                .Options);

        readonly BookingService bookingService;

        public FlightApplicationSpecifications()
        {
            bookingService = new BookingService(entities: entities);
        }


        [Theory]
        [InlineData("dam@net.com", 2)]
        [InlineData("don@net.com", 2)]
        public void Books_flights_Remembers_bookings(string passengerEmail, int numbersOfSeats)
        {

            var flight = new Flight(3);

            entities.Flights.Add(flight);

            bookingService.Book(new BookDto(
                flightId: flight.Id, passengerEmail, numbersOfSeats));

            bookingService.FindBookings(flight.Id).Should().ContainEquivalentOf(
                new BookingRm(passengerEmail, numbersOfSeats)
                );
        }

        [Theory]
        [InlineData(3)]
        [InlineData(10)]
        public void Cancels_booking_frees_up_seats(int initialCapacity)
        {

            var flight = new Flight(3);
            entities.Flights.Add(flight);

            bookingService.Book(new BookDto(flightId: flight.Id,
                passengerEmail: "dat@sud.com",
                numbersOfSeats: 2));

            bookingService.CancelBooking(
            new CancelBookingDto(
                flightId: flight.Id,
                passengerEmail: "dat@sud.com",
                numberOfSeats: 2)
            );

            bookingService.GetRemainingNumberOfSeatersFor(flight.Id).Should().Be(initialCapacity);
        }
    }

}