using Xunit;
using FluentAssertions;
using Domain;

namespace FlightTest
{
    public class Flight_test
    {
        [Theory]
        [InlineData(3,1,2)]
        [InlineData(6, 3, 3)]
        [InlineData(10, 6, 4)]
        public void Booking_reduces_the_number_of_seats(int seatCapacity, int numberOfSeats, int remainingNumberOfSeats)
        {
            var flight = new Flight(seatCapacity: seatCapacity);

            flight.Book("damton@yahoo.com", numberOfSeats);

            flight.RemainingNumberOfSeats.Should().Be(remainingNumberOfSeats);
        }

        [Fact]

        public void Avoids_overbooking()
        {
            var flight = new Flight(seatCapacity: 3);

            var error = flight.Book("damton@yahoo.com", 4);

            error.Should().BeOfType<OverbookingError>();
        }



        [Fact]
        public void Books_flights_successfully()
        {
            var flight = new Flight(seatCapacity: 3);

            var error = flight.Book("damton@yahoo.com", 1);

            error.Should().BeNull();
        }

        [Theory]
        [InlineData(3,1,1,3)]
        [InlineData(4, 2, 2, 4)]
        [InlineData(7, 5, 4, 6)]

        public void Canceling_bookings_frees_up_seats(
            int initialCapacity,
            int numberOfSeatsToBook,
            int numberOfSeatsToCancel,
            int remainingNumberOfSeats
            )
        {
            var flight = new Flight(initialCapacity);

            flight.Book(passengerEmail: "dav@gmail.com", numberOfSeats: numberOfSeatsToBook);

            flight.CancelBooking(passengerEmail: "dav@gmail.com", numberOfSeats: numberOfSeatsToCancel);
           
            flight.RemainingNumberOfSeats.Should().Be(remainingNumberOfSeats);
        }

        [Fact]
        public void Remembers_bookings()
        {
            var flight = new Flight(seatCapacity: 150);

            flight.Book(passengerEmail: "dav@gmail.com", numberOfSeats: 4);

            flight.BookingList.Should().ContainEquivalentOf(new Booking("dav@gmail.com", 4));
        }

        [Fact]
        public void Doesnt_cancel_bookings_for_passengers_who_have_not_booked()
        {
            var flight = new Flight(5);

            var error = flight.CancelBooking(passengerEmail: "dav@gmail.com", numberOfSeats: 1);

            error.Should().BeOfType<BookingNotFoundError>();
        }

        [Fact]
        public void Returns_null_when_successfully_cancel_a_booking()
        {
            var flight = new Flight(3);

            flight.Book(passengerEmail: "dav@gmail.com", numberOfSeats: 2);

            var error = flight.CancelBooking(passengerEmail: "dav@gmail.com", numberOfSeats: 1);
            
            error.Should().BeNull();
        }
    }
}
