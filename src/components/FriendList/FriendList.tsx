import React from 'react';
import './FriendList.css';

interface Friend {
  id: number;
  firstName: string;
  lastName: string;
  profileImage?: string;
  status: 'online' | 'offline';
  lastActive?: string;
}

const FriendList = () => {
  // This will be replaced with actual data from your backend
  const friends: Friend[] = [
    {
      id: 1,
      firstName: 'John',
      lastName: 'Doe',
      status: 'online',
      profileImage: '/default-avatar.png'
    },
    {
      id: 2,
      firstName: 'Jane',
      lastName: 'Smith',
      status: 'offline',
      lastActive: '2 hours ago',
      profileImage: '/default-avatar.png'
    }
  ];

  return (
    <div className="friend-list">
      <h2>Friends</h2>
      <div className="friend-list-container">
        {friends.map((friend) => (
          <div key={friend.id} className="friend-card">
            <div className="friend-image">
              <img src={friend.profileImage} alt={`${friend.firstName} ${friend.lastName}`} />
              <span className={`status-indicator ${friend.status}`}></span>
            </div>
            <div className="friend-info">
              <h3>{`${friend.firstName} ${friend.lastName}`}</h3>
              <p className="status-text">
                {friend.status === 'online' ? 'Online' : `Last seen: ${friend.lastActive}`}
              </p>
            </div>
            <div className="friend-actions">
              <button className="message-btn">Message</button>
              <button className="remove-btn">Remove</button>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default FriendList;